using System.Security.Claims;
using Authentication.Application.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace Authentication.Infrastructure.Authorization;

public sealed class PermissionAuthorizationHandler
    : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        if (!context.User.Identity?.IsAuthenticated ?? true)
        {
            return Task.CompletedTask;
        }

        var role = context.User.FindFirst(
            ClaimTypes.Role)?.Value;

        if (role is null)
        {
            return Task.CompletedTask;
        }

        if (RolePermissions.HasPermission(
                role,
                requirement.Permission))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}