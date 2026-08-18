namespace Authentication.Application.Authorization;

public static class RolePermissions
{
    private static readonly IReadOnlyDictionary<string, HashSet<string>>
        PermissionsByRole =
            new Dictionary<string, HashSet<string>>(
                StringComparer.OrdinalIgnoreCase)
            {
                ["Admin"] =
                [
                    Permissions.UsersRead,
                    Permissions.UsersCreate,
                    Permissions.UsersUpdate,
                    Permissions.UsersDelete
                ],

                ["User"] =
                [
                    Permissions.UsersRead
                ]
            };

    public static bool HasPermission(
        string role,
        string permission)
    {
        return PermissionsByRole.TryGetValue(
                   role,
                   out var permissions)
               && permissions.Contains(permission);
    }
}