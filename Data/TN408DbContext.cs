using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using THUD_TN408.Models;

namespace THUD_TN408.Data
{
	public class TN408DbContext : IdentityDbContext<User>
	{
		public TN408DbContext(DbContextOptions<TN408DbContext> options) : base(options)
		{ }

		public DbSet<Product> Products { get; set; }
		public DbSet<ProductDetail> Details { get; set; }
		public DbSet<Category> Categories { get; set; }
		public DbSet<Cart> Carts { get; set; }
		public DbSet<Order> Orders { get; set; }
		public DbSet<Payment> Payments { get; set; }
		public DbSet<Price> Prices { get; set; }
		public DbSet<Warehouse> Warehouses { get; set; }
		public DbSet<WarehouseDetail> WarehouseDetails { get; set; }

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);
			builder.HasDefaultSchema("dbo");

			builder.Entity<User>(user =>
			{
				user.ToTable(name: "User");
				user.Property("FirstName").HasMaxLength(25);
				user.Property("LastName").HasMaxLength(20);
				user.Property("Address").IsRequired(false).HasMaxLength(100);
				user.Property("DateOfBirth").IsRequired(false).HasColumnName("DOB");
				user.HasMany<Cart>(c => c.Carts).WithOne(c => c.User);
				user.HasMany<Order>(c => c.Orders).WithOne(o => o.User);
			});

			builder.Entity<Product>(product =>
			{
				product.HasKey(p => p.Id);
				product.Property("Id").ValueGeneratedOnAdd();
				product.Property("Name").HasMaxLength(30);
				product.Property("Origin").HasMaxLength(15);
				product.Property("Description").HasMaxLength(500);
				product.Property("IsActive").HasDefaultValue(true);
				product.HasMany<ProductDetail>(p => p.Details).WithOne(d => d.Product);
			});

			builder.Entity<ProductDetail>(detail =>
			{
				detail.ToTable("ProductDetails");
				detail.HasKey(p => p.Id);
				detail.Property("Id").ValueGeneratedOnAdd();
				detail.Property("Size").HasMaxLength(6);
				detail.Property("Gender").HasDefaultValue(true);
				detail.Property("Color").HasMaxLength(20);
				detail.Property("Image1").HasMaxLength(50);
				detail.Property("Image2").HasMaxLength(50);
				detail.HasMany<Cart>(d => d.Carts).WithOne(c => c.Detail);
				detail.HasMany<Price>(d => d.Prices).WithOne(p => p.Detail);
			});

			builder.Entity<Warehouse>(warehouse =>
			{
				warehouse.HasKey(w => w.Id);
				warehouse.Property("Id").ValueGeneratedOnAdd();
				warehouse.Property("Name").HasMaxLength(30);
				warehouse.Property("Address").HasMaxLength(100);
			});

			builder.Entity<WarehouseDetail>(wDetail =>
			{
				wDetail.HasKey(wd => new { wd.WarehouseId, wd.ProductDetailId});
				wDetail.HasOne<ProductDetail>(wd => wd.ProductDetail).WithMany(d => d.StockDetails);
				wDetail.HasOne<Warehouse>(wd => wd.Warehouse).WithMany(w => w.Details);
			});

			builder.Entity<Category>(category =>
			{
				category.HasKey(category => category.Id);
				category.Property("Id").ValueGeneratedOnAdd();
				category.Property("Name").HasMaxLength(20);
				category.Property("IsActive").HasDefaultValue(true);
				category.HasMany<Product>(c => c.Products).WithOne(p => p.Category);
			});

			builder.Entity<Payment>(payment =>
			{
				payment.HasKey(p => p.Id);
				payment.Property("Id").ValueGeneratedOnAdd();
				payment.Property("Name").HasMaxLength(30);
				payment.HasMany<Order>(p => p.Orders).WithOne(o => o.Payment);
			});

			builder.Entity<Order>(order =>
			{
				order.HasKey(o => o.Id);
				order.Property("Id").ValueGeneratedOnAdd();
				order.Property("CreatedDate").HasDefaultValue(DateTime.UtcNow).HasColumnName("Created_at");
				order.Property("IsPaid").HasDefaultValue(false);
				order.Property("IsTrans").HasDefaultValue(false);
				order.Property("IsSuccess").HasDefaultValue(false);
				order.HasMany<Cart>(o => o.Carts).WithOne(c => c.Order).OnDelete(DeleteBehavior.NoAction);
			});

			builder.Entity<Cart>(cart =>
			{
				cart.HasKey(c => c.Id);
				cart.Property("Id").ValueGeneratedOnAdd();
				cart.Property("Quantity").HasDefaultValue(1);
				cart.Property("IsDeleted").HasDefaultValue(false);
				cart.Property("IsCheckedOut").HasDefaultValue(false);
			});

			builder.Entity<Price>(price =>
			{
				price.HasKey(p => p.Id);
				price.Property("Id").ValueGeneratedOnAdd();
				price.Property("DateApply").HasDefaultValue(DateTime.UtcNow);
			});


			builder.Entity<IdentityRole>(entity =>
			{
				entity.ToTable(name: "Role");
			});
			builder.Entity<IdentityUserRole<string>>(entity =>
			{
				entity.ToTable("UserRoles");
			});
			builder.Entity<IdentityUserClaim<string>>(entity =>
			{
				entity.ToTable("UserClaims");
			});
			builder.Entity<IdentityUserLogin<string>>(entity =>
			{
				entity.ToTable("UserLogins");
			});
			builder.Entity<IdentityRoleClaim<string>>(entity =>
			{
				entity.ToTable("RoleClaims");
			});
			builder.Entity<IdentityUserToken<string>>(entity =>
			{
				entity.ToTable("UserTokens");
			});

			//builder.Seed();
		}
	}
}