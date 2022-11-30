using Microsoft.AspNetCore.Authorization;

namespace THUD_TN408.Permission
{
	internal class PermissionRequirement : IAuthorizationRequirement
	{
		public string Permission { get; private set; }

		public PermissionRequirement(string permission)
		{
			Permission = permission;
		}
	}
}
