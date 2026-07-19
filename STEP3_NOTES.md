# Step 3 — Dynamic Menu — Run Notes

## What was added

- `Models/MenuItem.cs` and `Models/MenuRolePermission.cs` already existed from Step 1.
- **New DTOs** under `DTOs/Menu/`:
  - `MenuItemTreeDto` — nested response for `GET /api/menu`
  - `MenuItemAdminDto` — flat list with role IDs (admin UI)
  - `CreateMenuItemDto`, `UpdateMenuItemDto`, `AssignMenuRolesDto`
- **`Services/MenuService.cs`** — `IMenuService` + implementation
  - `GetMenuForRoleAsync(roleId)` builds the nested tree in one DB hit
  - SuperAdmin CRUD plus role-permission replacement
- **`Controllers/MenuController.cs`**
  - `GET /api/menu` — the task-doc endpoint, returns the tree filtered to the
    caller's role (role ID read from the `roleId` JWT claim that `JwtHelper`
    already issues)
  - `GET /api/menu/all`, `POST`, `PUT/{id}`, `DELETE/{id}`, `PUT/{id}/roles` —
    all `[Authorize(Roles = "superadmin")]`
- **`Data/AppDbContext.cs`** — added `HasData` seeds for **19 menu items** and
  **65 role-permission rows** matching the *Module Access by Role* matrix in
  the task doc.
- **`Program.cs`** — registered `IMenuService` in DI.

## Apply the new seed to your DB

Because there are new `HasData` rows, you need a fresh EF migration.

```powershell
cd "D:\School System\SchoolManagement.API"
dotnet ef migrations add SeedDynamicMenu
dotnet ef database update
```

If you have `app.Environment.IsDevelopment()` auto-migrate on startup (you do
in `Program.cs`), running the app once will also apply it.

## Smoke-test it

1. Log in as `superadmin@school.com` / `Admin@123` via `POST /api/auth/login`.
2. Hit `GET /api/menu` with the returned `Bearer` token — you'll get the full
   tree (Dashboard, Users, Academics, Students, Timetable, Attendance,
   Homework, Files, Menu Management).
3. Create another user with `RoleId = 4` (teacher) via `POST /api/users`,
   log in as them, and hit `GET /api/menu` again — you'll see only:
   - Dashboard, Timetable (with View Timetable), Attendance (Mark + View),
     Homework (Assign + Diary), Files.

## Endpoints summary

| Verb   | Route                       | Auth                 |
|--------|-----------------------------|----------------------|
| GET    | `/api/menu`                 | Any authenticated    |
| GET    | `/api/menu/all`             | superadmin           |
| POST   | `/api/menu`                 | superadmin           |
| PUT    | `/api/menu/{id}`            | superadmin           |
| DELETE | `/api/menu/{id}`            | superadmin (no children) |
| PUT    | `/api/menu/{id}/roles`      | superadmin           |

## Why the role check uses `superadmin` (lowercase)

The Step 1 seed inserts role names in lowercase (`superadmin`, `admin`, etc.)
and `JwtHelper` puts the role *name* in the JWT role claim. `[Authorize(Roles = "...")]`
matches case-sensitively, so the controller uses the lowercase forms to align
with the actual JWT contents. The `roleId` numeric claim is read separately to
filter the menu.
