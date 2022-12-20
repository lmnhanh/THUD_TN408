namespace THUD_TN408.Authorization
{
    public static class Permissions
    {
		public static List<string> GeneratePermissionsForModule(string module)
		{
			return new List<string>()
				{
					$"Permissions.{module}.Create",
					$"Permissions.{module}.View",
					$"Permissions.{module}.Edit",
					$"Permissions.{module}.Delete",
				};
		}
		public static class Products
		{
			public const string View = "Permissions.Products.View";
			public const string Create = "Permissions.Products.Create";
			public const string Edit = "Permissions.Products.Edit";
			public const string Delete = "Permissions.Products.Delete";
		}
		public static class Producers
		{
			public const string View = "Permissions.Producers.View";
			public const string Create = "Permissions.Producers.Create";
			public const string Edit = "Permissions.Producers.Edit";
			public const string Delete = "Permissions.Producers.Delete";
		}

		public static class Categories
		{
			public const string View = "Permissions.Categories.View";
			public const string Create = "Permissions.Categories.Create";
			public const string Edit = "Permissions.Categories.Edit";
			public const string Delete = "Permissions.Categories.Delete";
		}

		public static class StaffAccounts
		{
			public const string View = "Permissions.StaffAccounts.View";
			public const string Create = "Permissions.StaffAccounts.Create";
			public const string Edit = "Permissions.StaffAccounts.Edit";
			public const string Delete = "Permissions.StaffAccounts.Delete";
		}

		public static class CustomerAccounts
		{
			public const string View = "Permissions.CustomerAccounts.View";
			public const string Create = "Permissions.CustomerAccounts.Create";
			public const string Edit = "Permissions.CustomerAccounts.Edit";
			public const string Delete = "Permissions.CustomerAccounts.Delete";
		}

		public static class Orders
		{
			public const string View = "Permissions.Orders.View";
			public const string Create = "Permissions.Orders.Create";
			public const string Edit = "Permissions.Orders.Edit";
			public const string Delete = "Permissions.Orders.Delete";
		}
		public static class Promotions
		{
			public const string View = "Permissions.Promotions.View";
			public const string Create = "Permissions.Promotions.Create";
			public const string Edit = "Permissions.Promotions.Edit";
			public const string Delete = "Permissions.Promotions.Delete";
		}
		public static class Carts
		{
			public const string View = "Permissions.Carts.View";
			public const string Create = "Permissions.Carts.Create";
			public const string Edit = "Permissions.Carts.Edit";
			public const string Delete = "Permissions.Carts.Delete";
		}

		public static class Warehouses
		{
			public const string View = "Permissions.Warehouses.View";
			public const string Create = "Permissions.Warehouses.Create";
			public const string Edit = "Permissions.Warehouses.Edit";
			public const string Delete = "Permissions.Warehouses.Delete";
		}

		public static class Payments
		{
			public const string View = "Permissions.Payments.View";
			public const string Create = "Permissions.Payments.Create";
			public const string Edit = "Permissions.Payments.Edit";
			public const string Delete = "Permissions.Payments.Delete";
		}

		public static class Shipments
		{
			public const string View = "Permissions.Shipments.View";
			public const string Create = "Permissions.Shipments.Create";
			public const string Edit = "Permissions.Shipments.Edit";
			public const string Delete = "Permissions.Shipments.Delete";
		}

		public static class ImportNotes
		{
			public const string View = "Permissions.ImportNotes.View";
			public const string Create = "Permissions.ImportNotes.Create";
			public const string Edit = "Permissions.ImportNotes.Edit";
			public const string Delete = "Permissions.ImportNotes.Delete";
		}
	}
}
