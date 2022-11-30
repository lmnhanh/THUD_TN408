using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using THUD_TN408.Permission;

namespace THUD_TN408.Authorization
{
	internal class PermissionPolicyProvider : IAuthorizationPolicyProvider
	{
		public DefaultAuthorizationPolicyProvider FallbackPolicyProvider { get; }

		public PermissionPolicyProvider(IOptions<AuthorizationOptions> options)
		{
			FallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
		}

		public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => FallbackPolicyProvider.GetDefaultPolicyAsync();

		public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
		{
			if (policyName.StartsWith("Permission", StringComparison.OrdinalIgnoreCase))
			{
				var policy = new AuthorizationPolicyBuilder();
				policy.AddRequirements(new PermissionRequirement(policyName));
				return Task.FromResult(policy.Build());
			}

			// Policy is not for permissions, try the default provider.
			return FallbackPolicyProvider.GetPolicyAsync(policyName);
		}

		public Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
		{
			return ((IAuthorizationPolicyProvider)FallbackPolicyProvider).GetFallbackPolicyAsync();
		}
	}
}
