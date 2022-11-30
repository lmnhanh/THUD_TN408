using Microsoft.AspNetCore.Authorization;
using THUD_TN408.Permission;

namespace THUD_TN408.Authorization
{
	internal class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
	{
		public PermissionAuthorizationHandler()
		{

		}
		protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
		{
			if (context.User == null)
			{
				return;
			}
			var permissionss = context.User.Claims.Where(x => x.Type == "Permission" && x.Value == requirement.Permission && x.Issuer == "LOCAL AUTHORITY");
			if (permissionss.Any())
			{
				context.Succeed(requirement);
				return;
			}

			return;
		}
	}
}
