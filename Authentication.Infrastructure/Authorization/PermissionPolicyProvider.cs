using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Authentication.Infrastructure.Authorization;

public sealed class PermissionPolicyProvider
    : DefaultAuthorizationPolicyProvider
{
    public const string PolicyPrefix = "Permission:";

    public PermissionPolicyProvider(
        IOptions<AuthorizationOptions> options)
        : base(options)
    {
    }

    public override Task<AuthorizationPolicy?> GetPolicyAsync(
        string policyName)
    {
        if (policyName.StartsWith(
                PolicyPrefix,
                StringComparison.OrdinalIgnoreCase))
        {
            var permission = policyName[
                PolicyPrefix.Length..];

            var policy = new AuthorizationPolicyBuilder()
                .AddRequirements(
                    new PermissionRequirement(permission))
                .Build();

            return Task.FromResult<AuthorizationPolicy?>(
                policy);
        }

        return base.GetPolicyAsync(policyName);
    }
}