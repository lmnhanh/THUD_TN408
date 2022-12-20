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

		public DbSet<Product> Products { get; set; } = null!;
		public DbSet<ProductDetail> Details { get; set; } = null!;
		public DbSet<Category> Categories { get; set; } = null!;
		public DbSet<Cart> Carts { get; set; } = null!;
		public DbSet<Order> Orders { get; set; } = null!;
		public DbSet<Payment> Payments { get; set; } = null!;
		public DbSet<Price> Prices { get; set; } = null!;
		public DbSet<Warehouse> Warehouses { get; set; } = null!;
		public DbSet<WarehouseDetail> WarehouseDetails { get; set; } = null!;
		public DbSet<History> Histories { get; set; } = null!;
		public DbSet<ProductPromotion> PPromotions { get; set; } = null!;
		public DbSet<OrderPromotion> OPromotions { get; set; } = null!;
		public DbSet<ImportNote> ImportNotes { get; set; } = null!;
		public DbSet<ImportDetail> ImportDetails { get; set; } = null!;
		public DbSet<Producer> Producers { get; set; } = null!;
		public DbSet<Shipment> Shipments { get; set; } = null!;


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
				user.Property("DateOfBirth").HasColumnName("DOB");
				user.HasMany<Cart>(u => u.Carts).WithOne(c => c.User).HasForeignKey(c => c.UserId);
				user.HasMany<Order>(u => u.Orders).WithOne(o => o.User).HasForeignKey(o => o.UserId);
				user.HasMany<History>(u => u.Histories).WithOne(h => h.User).HasForeignKey(h => h.UserId);
				user.HasMany<ImportNote>(u => u.ImportNotes).WithOne(i => i.User).HasForeignKey(i => i.UserId);
			});

			builder.Entity<Product>(product =>
			{
				product.HasKey(p => p.Id);
				product.Property("Id").ValueGeneratedOnAdd();
				product.Property("Name").HasMaxLength(40);
				product.Property("Origin").HasMaxLength(15);
				product.Property("Description").HasMaxLength(500);
				product.HasMany<ProductDetail>(p => p.Details).WithOne(d => d.Product);
				product.HasMany<ProductPromotion>(d => d.Promotions).WithOne(p => p.Product).HasForeignKey(p => p.ProductId);
			});

			builder.Entity<Producer>(producer =>
			{
				producer.HasKey(producer => producer.Id);
				producer.Property("Id").ValueGeneratedOnAdd();
				producer.Property("Name").HasMaxLength(40);
				producer.Property("PhoneNumber").HasMaxLength(15);
				producer.Property("Email").HasMaxLength(100);
				producer.HasMany<Product>(pr => pr.Products).WithOne(p => p.Producer).HasForeignKey(p => p.ProducerId);
			});

			builder.Entity<ProductDetail>(detail =>
			{
				detail.ToTable("ProductDetails");
				detail.HasKey(p => p.Id);
				detail.Property("Id").ValueGeneratedOnAdd();
				detail.Property("Size").HasMaxLength(6);
				detail.Property("Color").HasMaxLength(20);
				detail.Property("Image1").HasMaxLength(50);
				detail.Property("Image2").HasMaxLength(50);
				detail.HasMany<Cart>(d => d.Carts).WithOne(c => c.Detail);
				detail.HasMany<Price>(d => d.Prices).WithOne(p => p.Detail);
				detail.HasMany<ImportDetail>(d => d.ImportDetails).WithOne(i => i.ProductDetail).HasForeignKey(i => i.ProductDetailId);
			});

			builder.Entity<Warehouse>(warehouse =>
			{
				warehouse.HasKey(w => w.Id);
				warehouse.Property("Id").ValueGeneratedOnAdd();
				warehouse.Property("Name").HasMaxLength(30);
				warehouse.Property("Address").HasMaxLength(100);
				warehouse.HasMany<ImportNote>(wh => wh.ImportNotes).WithOne(i => i.Warehouse).HasForeignKey(i => i.WarehouseId);
			});

			builder.Entity<WarehouseDetail>(wDetail =>
			{
				wDetail.HasKey(wd => new { wd.WarehouseId, wd.ProductDetailId});
				wDetail.HasOne<ProductDetail>(wd => wd.ProductDetail).WithMany(d => d.StockDetails);
				wDetail.HasOne<Warehouse>(wd => wd.Warehouse).WithMany(w => w.Details);
			});

			builder.Entity<ImportNote>(note =>
			{
				note.HasKey(note => note.Id);
				note.Property("Id").ValueGeneratedOnAdd();
			});

			builder.Entity<ImportDetail>(iDetail =>
			{
				iDetail.HasKey(id => new {id.ImportNoteId, id.ProductDetailId });
				iDetail.HasOne<ProductDetail>(id => id.ProductDetail).WithMany(d => d.ImportDetails);
				iDetail.HasOne<ImportNote>(id => id.ImportNote).WithMany(i => i.Details);
			});

			builder.Entity<Category>(category =>
			{
				category.HasKey(category => category.Id);
				category.Property("Id").ValueGeneratedOnAdd();
				category.Property("Name").HasMaxLength(20);
				category.HasMany<Product>(c => c.Products).WithOne(p => p.Category);
			});

			builder.Entity<Payment>(payment =>
			{
				payment.HasKey(p => p.Id);
				payment.Property("Id").ValueGeneratedOnAdd();
				payment.Property("Name").HasMaxLength(30);
				payment.HasMany<Order>(p => p.Orders).WithOne(o => o.Payment).HasForeignKey(o => o.PaymentId);
			});

			builder.Entity<Shipment>(shipment =>
			{
				shipment.HasKey(p => p.Id);
				shipment.Property("Id").ValueGeneratedOnAdd();
				shipment.Property("Name").HasMaxLength(30);
				shipment.HasMany<Order>(p => p.Orders).WithOne(o => o.Shipment).HasForeignKey(o => o.ShipmentId);
			});

			builder.Entity<Order>(order =>
			{
				order.HasKey(o => o.Id);
				order.Property("Id").ValueGeneratedOnAdd();
				order.Property("CreatedDate").HasColumnName("Created_at");
				order.Property("IsTrans").HasDefaultValue(false);
				order.Property("IsSuccess").HasDefaultValue(false);
				order.HasMany<Cart>(o => o.Carts).WithOne(c => c.Order).OnDelete(DeleteBehavior.NoAction);
			});

			builder.Entity<Cart>(cart =>
			{
				cart.HasKey(c => c.Id);
				cart.Property("Id").ValueGeneratedOnAdd();
				cart.Property("IsCheckedOut").HasDefaultValue(false);
				cart.Property("OrderId").IsRequired(false);
			});

			builder.Entity<Price>(price =>
			{
				price.HasKey(p => p.Id);
				price.Property("Id").ValueGeneratedOnAdd();
			});

			builder.Entity<History>(history =>
			{
				history.HasKey(h => h.Id);
				history.Property("Id").ValueGeneratedOnAdd();
				history.Property("CreatedAt").HasDefaultValue(DateTime.Now);
				history.Property("Message").HasMaxLength(100).IsRequired(true);
				history.Property("TargetUrl").HasMaxLength(100).IsRequired(false);
			});

			builder.Entity<ProductPromotion>(ppromotion =>
			{
				ppromotion.HasKey(p => p.Id);
				ppromotion.Property("Id").HasMaxLength(12).IsRequired(true);
				ppromotion.Property("Name").HasMaxLength(30).IsRequired(true);
				ppromotion.Property("Description").HasMaxLength(100).IsRequired(true);
			});

			builder.Entity<OrderPromotion>(opromotion =>
			{
				opromotion.HasKey(p => p.Id);
				opromotion.Property("Id").HasMaxLength(12).IsRequired(true);
				opromotion.Property("Name").HasMaxLength(30).IsRequired(true);
				opromotion.Property("Description").HasMaxLength(100).IsRequired(true);
				opromotion.Property("MaxDiscount").IsRequired(false);
				opromotion.Property("ApplyCondition").IsRequired(false);
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

			builder.Seed();
		}
	}
}