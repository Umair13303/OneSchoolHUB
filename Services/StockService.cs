using Microsoft.EntityFrameworkCore;
using SchoolManagement.API.Data;
using SchoolManagement.API.DTOs.Inventory;
using SchoolManagement.API.Models;

namespace SchoolManagement.API.Services;

/// <summary>
/// Core stock engine shared by Purchase, POS, Returns, Adjustment and Transfer
/// services. Every stock-affecting transaction in the module must go through
/// <see cref="PostMovementAsync"/> so that StockLedger (audit trail) and
/// CurrentStock (fast running balance) never drift apart.
/// NOTE: callers must SaveChangesAsync on the shared DbContext themselves —
/// this service adds tracked entities but does not commit, so a purchase/sale
/// and its stock movements commit together in one transaction.
/// </summary>
public interface IStockService
{
    Task<decimal> PostMovementAsync(int itemId, string voucherType, string voucherNo, decimal qtyIn, decimal qtyOut, decimal cost, int userId, DateTime? transactionDate = null);
    Task<decimal> GetQuantityOnHandAsync(int itemId);
    Task<bool> IsNegativeStockAllowedAsync();
    Task ApplyPurchaseCostingAsync(ItemMaster item, decimal quantity, decimal purchasePrice);

    Task<List<CurrentStockDto>> GetCurrentStockAsync(int? categoryId, string? search);
    Task<List<CurrentStockDto>> GetLowStockAsync();
    Task<List<CurrentStockDto>> GetOutOfStockAsync();
    Task<List<StockValuationRowDto>> GetStockValuationAsync();
    Task<List<StockLedgerDto>> GetLedgerAsync(int? itemId, DateTime? from, DateTime? to);

    Task<StockAdjustmentDto> CreateAdjustmentAsync(CreateStockAdjustmentDto dto, int userId);
    Task<List<StockAdjustmentDto>> GetAdjustmentsAsync();

    Task<StockTransferDto> CreateTransferAsync(CreateStockTransferDto dto, int userId);
    Task<List<StockTransferDto>> GetTransfersAsync();

    Task<string> NextVoucherNoAsync(string prefix, Func<Task<int>> countQuery);
}

public class StockService : IStockService
{
    private readonly AppDbContext _db;
    public StockService(AppDbContext db) => _db = db;

    // ── Core movement posting ────────────────────────────────────────────────
    public async Task<decimal> PostMovementAsync(int itemId, string voucherType, string voucherNo, decimal qtyIn, decimal qtyOut, decimal cost, int userId, DateTime? transactionDate = null)
    {
        var current = await _db.CurrentStocks.FirstOrDefaultAsync(c => c.ItemId == itemId);
        var before = current?.QuantityOnHand ?? 0m;
        var after = before + qtyIn - qtyOut;

        if (after < 0 && !await IsNegativeStockAllowedAsync())
            throw new InvalidOperationException($"Insufficient stock for item {itemId}. Available: {before}, requested: {qtyOut - qtyIn}.");

        if (current is null)
        {
            current = new CurrentStock { ItemId = itemId, QuantityOnHand = after, LastUpdated = DateTime.UtcNow };
            _db.CurrentStocks.Add(current);
        }
        else
        {
            current.QuantityOnHand = after;
            current.LastUpdated = DateTime.UtcNow;
        }

        _db.StockLedgers.Add(new StockLedger
        {
            TransactionDate = transactionDate ?? DateTime.UtcNow,
            VoucherType = voucherType,
            VoucherNo = voucherNo,
            ItemId = itemId,
            QtyIn = qtyIn,
            QtyOut = qtyOut,
            Balance = after,
            Cost = cost,
            UserId = userId
        });

        return after;
    }

    public async Task<decimal> GetQuantityOnHandAsync(int itemId)
        => (await _db.CurrentStocks.AsNoTracking().FirstOrDefaultAsync(c => c.ItemId == itemId))?.QuantityOnHand ?? 0m;

    public async Task<bool> IsNegativeStockAllowedAsync()
    {
        var s = await _db.InventorySettings.AsNoTracking().FirstOrDefaultAsync();
        return s?.NegativeStockAllowed ?? false;
    }

    /// <summary>Applies the configured costing method (Last Purchase Cost or Weighted Average) after a purchase line.</summary>
    public async Task ApplyPurchaseCostingAsync(ItemMaster item, decimal quantity, decimal purchasePrice)
    {
        var settings = await _db.InventorySettings.AsNoTracking().FirstOrDefaultAsync();
        var method = settings?.CostingMethod ?? "WeightedAverage";

        item.LastPurchasePrice = purchasePrice;

        if (method == "LastPurchaseCost")
        {
            item.AverageCost = purchasePrice;
        }
        else // WeightedAverage
        {
            var existingQty = await GetQuantityOnHandAsync(item.ItemId);
            var existingValue = existingQty * item.AverageCost;
            var incomingValue = quantity * purchasePrice;
            var totalQty = existingQty + quantity;
            item.AverageCost = totalQty > 0 ? Math.Round((existingValue + incomingValue) / totalQty, 4) : purchasePrice;
        }
    }

    // ── Queries ───────────────────────────────────────────────────────────────
    public async Task<List<CurrentStockDto>> GetCurrentStockAsync(int? categoryId, string? search)
    {
        var q = _db.ItemMasters.AsNoTracking().Where(i => i.IsActive).AsQueryable();
        if (categoryId.HasValue) q = q.Where(i => i.ItemCategoryId == categoryId.Value);
        if (!string.IsNullOrWhiteSpace(search))
            q = q.Where(i => i.ItemName.Contains(search) || i.ItemCode.Contains(search) || (i.Barcode != null && i.Barcode.Contains(search)));

        var items = await q.Include(i => i.Category).ToListAsync();
        var stockMap = await _db.CurrentStocks.AsNoTracking().ToDictionaryAsync(c => c.ItemId, c => c);

        return items.Select(i =>
        {
            stockMap.TryGetValue(i.ItemId, out var cs);
            return new CurrentStockDto
            {
                ItemId = i.ItemId,
                ItemCode = i.ItemCode,
                ItemName = i.ItemName,
                Barcode = i.Barcode,
                CategoryName = i.Category?.CategoryName ?? string.Empty,
                QuantityOnHand = cs?.QuantityOnHand ?? 0m,
                MinimumStockLevel = i.MinimumStockLevel,
                ReorderLevel = i.ReorderLevel,
                AverageCost = i.AverageCost,
                StockValue = (cs?.QuantityOnHand ?? 0m) * i.AverageCost,
                LastUpdated = cs?.LastUpdated ?? i.CreatedAt
            };
        }).OrderBy(d => d.ItemName).ToList();
    }

    public async Task<List<CurrentStockDto>> GetLowStockAsync()
    {
        var all = await GetCurrentStockAsync(null, null);
        return all.Where(s => s.QuantityOnHand <= s.ReorderLevel && s.QuantityOnHand > 0).ToList();
    }

    public async Task<List<CurrentStockDto>> GetOutOfStockAsync()
    {
        var all = await GetCurrentStockAsync(null, null);
        return all.Where(s => s.QuantityOnHand <= 0).ToList();
    }

    public async Task<List<StockValuationRowDto>> GetStockValuationAsync()
    {
        var all = await GetCurrentStockAsync(null, null);
        return all.Select(s => new StockValuationRowDto
        {
            ItemName = s.ItemName,
            CategoryName = s.CategoryName,
            QuantityOnHand = s.QuantityOnHand,
            AverageCost = s.AverageCost,
            StockValue = s.StockValue
        }).ToList();
    }

    public async Task<List<StockLedgerDto>> GetLedgerAsync(int? itemId, DateTime? from, DateTime? to)
    {
        var q = _db.StockLedgers.AsNoTracking().Include(l => l.Item).AsQueryable();
        if (itemId.HasValue) q = q.Where(l => l.ItemId == itemId.Value);
        if (from.HasValue) q = q.Where(l => l.TransactionDate >= from.Value);
        if (to.HasValue) q = q.Where(l => l.TransactionDate <= to.Value);

        var rows = await q.OrderByDescending(l => l.TransactionDate).ThenByDescending(l => l.StockLedgerId).Take(2000).ToListAsync();
        var userNames = await _db.Users.AsNoTracking()
            .Where(u => rows.Select(r => r.UserId).Contains(u.UserId))
            .ToDictionaryAsync(u => u.UserId, u => u.FullName);

        return rows.Select(l => new StockLedgerDto
        {
            StockLedgerId = l.StockLedgerId,
            TransactionDate = l.TransactionDate,
            VoucherType = l.VoucherType,
            VoucherNo = l.VoucherNo,
            ItemId = l.ItemId,
            ItemName = l.Item.ItemName,
            QtyIn = l.QtyIn,
            QtyOut = l.QtyOut,
            Balance = l.Balance,
            Cost = l.Cost,
            UserName = userNames.TryGetValue(l.UserId, out var n) ? n : string.Empty
        }).ToList();
    }

    // ── Adjustments (Adjustment / Damage / Expired / Physical Verification) ──
    public async Task<StockAdjustmentDto> CreateAdjustmentAsync(CreateStockAdjustmentDto dto, int userId)
    {
        if (dto.Lines.Count == 0) throw new ArgumentException("At least one line is required.");

        var no = await NextVoucherNoAsync("ADJ", () => _db.StockAdjustmentMasters.IgnoreQueryFilters().CountAsync());
        var master = new StockAdjustmentMaster
        {
            AdjustmentNo = no,
            AdjustmentDate = dto.AdjustmentDate,
            AdjustmentType = dto.AdjustmentType,
            Remarks = dto.Remarks,
            Status = "Posted"
        };
        _db.StockAdjustmentMasters.Add(master);

        foreach (var line in dto.Lines)
        {
            var item = await _db.ItemMasters.FirstOrDefaultAsync(i => i.ItemId == line.ItemId)
                ?? throw new ArgumentException($"Item {line.ItemId} not found.");
            var before = await GetQuantityOnHandAsync(line.ItemId);
            var diff = line.NewQuantity - before;

            master.Details.Add(new StockAdjustmentDetail
            {
                ItemId = line.ItemId,
                QtyBefore = before,
                QtyAfter = line.NewQuantity,
                QtyDiff = diff,
                Cost = item.AverageCost
            });

            if (diff > 0)
                await PostMovementAsync(line.ItemId, dto.AdjustmentType, no, diff, 0, item.AverageCost, userId, dto.AdjustmentDate.ToDateTime(TimeOnly.MinValue));
            else if (diff < 0)
                await PostMovementAsync(line.ItemId, dto.AdjustmentType, no, 0, -diff, item.AverageCost, userId, dto.AdjustmentDate.ToDateTime(TimeOnly.MinValue));
        }

        await _db.SaveChangesAsync();
        return (await GetAdjustmentsAsync()).First(a => a.StockAdjustmentId == master.StockAdjustmentId);
    }

    public async Task<List<StockAdjustmentDto>> GetAdjustmentsAsync()
    {
        var list = await _db.StockAdjustmentMasters.AsNoTracking()
            .Include(a => a.Details).ThenInclude(d => d.Item)
            .OrderByDescending(a => a.AdjustmentDate).ThenByDescending(a => a.StockAdjustmentId)
            .ToListAsync();

        return list.Select(a => new StockAdjustmentDto
        {
            StockAdjustmentId = a.StockAdjustmentId,
            AdjustmentNo = a.AdjustmentNo,
            AdjustmentDate = a.AdjustmentDate,
            AdjustmentType = a.AdjustmentType,
            Remarks = a.Remarks,
            Status = a.Status,
            Details = a.Details.Select(d => new StockAdjustmentDetailDto
            {
                ItemId = d.ItemId,
                ItemName = d.Item.ItemName,
                QtyBefore = d.QtyBefore,
                QtyAfter = d.QtyAfter,
                QtyDiff = d.QtyDiff,
                Cost = d.Cost
            }).ToList()
        }).ToList();
    }

    // ── Transfers ────────────────────────────────────────────────────────────
    // Single aggregate stock pool per institute (no Location master yet — see
    // "Multiple store locations" future enhancement), so a transfer is recorded
    // as a net-zero audit trail: a QtyOut line followed immediately by a QtyIn
    // line under the same voucher number, rather than moving quantity between
    // two separately-tracked balances.
    public async Task<StockTransferDto> CreateTransferAsync(CreateStockTransferDto dto, int userId)
    {
        if (dto.Lines.Count == 0) throw new ArgumentException("At least one line is required.");

        var no = await NextVoucherNoAsync("TRF", () => _db.StockTransferMasters.IgnoreQueryFilters().CountAsync());
        var master = new StockTransferMaster
        {
            TransferNo = no,
            TransferDate = dto.TransferDate,
            FromLocation = dto.FromLocation,
            ToLocation = dto.ToLocation,
            Remarks = dto.Remarks,
            Status = "Posted"
        };
        _db.StockTransferMasters.Add(master);

        foreach (var line in dto.Lines)
        {
            var item = await _db.ItemMasters.FirstOrDefaultAsync(i => i.ItemId == line.ItemId)
                ?? throw new ArgumentException($"Item {line.ItemId} not found.");
            master.Details.Add(new StockTransferDetail { ItemId = line.ItemId, Quantity = line.Quantity });

            var transactionDate = dto.TransferDate.ToDateTime(TimeOnly.MinValue);
            await PostMovementAsync(line.ItemId, "Transfer", no, 0, line.Quantity, item.AverageCost, userId, transactionDate);
            await PostMovementAsync(line.ItemId, "Transfer", no, line.Quantity, 0, item.AverageCost, userId, transactionDate);
        }

        await _db.SaveChangesAsync();
        return (await GetTransfersAsync()).First(t => t.StockTransferId == master.StockTransferId);
    }

    public async Task<List<StockTransferDto>> GetTransfersAsync()
    {
        var list = await _db.StockTransferMasters.AsNoTracking()
            .Include(t => t.Details).ThenInclude(d => d.Item)
            .OrderByDescending(t => t.TransferDate).ThenByDescending(t => t.StockTransferId)
            .ToListAsync();

        return list.Select(t => new StockTransferDto
        {
            StockTransferId = t.StockTransferId,
            TransferNo = t.TransferNo,
            TransferDate = t.TransferDate,
            FromLocation = t.FromLocation,
            ToLocation = t.ToLocation,
            Remarks = t.Remarks,
            Status = t.Status,
            Details = t.Details.Select(d => new StockTransferDetailDto
            {
                ItemId = d.ItemId,
                ItemName = d.Item.ItemName,
                Quantity = d.Quantity
            }).ToList()
        }).ToList();
    }

    // ── Voucher numbering ────────────────────────────────────────────────────
    public async Task<string> NextVoucherNoAsync(string prefix, Func<Task<int>> countQuery)
    {
        var count = await countQuery();
        return $"{prefix}-{(count + 1):D6}";
    }
}
