-- Teacher Daily Classroom menu (existing databases).
-- Safe to re-run.

IF NOT EXISTS (SELECT 1 FROM MenuItems WHERE MenuItemId = 59)
BEGIN
    SET IDENTITY_INSERT MenuItems ON;
    INSERT INTO MenuItems (MenuItemId, ParentId, Title, Icon, RouteUrl, SortOrder, IsActive, IsDeleted, CreatedAt)
    VALUES (59, NULL, N'Classroom', N'play_lesson', N'/classroom/today', 12, 1, 0, SYSUTCDATETIME());
    SET IDENTITY_INSERT MenuItems OFF;
END
ELSE
BEGIN
    UPDATE MenuItems
    SET Title = N'Classroom',
        Icon = N'play_lesson',
        RouteUrl = N'/classroom/today',
        SortOrder = 12,
        IsActive = 1,
        IsDeleted = 0
    WHERE MenuItemId = 59;
END

IF NOT EXISTS (SELECT 1 FROM MenuRolePermissions WHERE Id = 214)
BEGIN
    SET IDENTITY_INSERT MenuRolePermissions ON;
    INSERT INTO MenuRolePermissions (Id, MenuItemId, RoleId)
    VALUES (214, 59, 4); -- teacher
    SET IDENTITY_INSERT MenuRolePermissions OFF;
END
ELSE
BEGIN
    UPDATE MenuRolePermissions SET MenuItemId = 59, RoleId = 4 WHERE Id = 214;
END
