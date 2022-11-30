using Microsoft.EntityFrameworkCore;
using THUD_TN408.Models;

namespace THUD_TN408.Data
{
	public static class DatabaseInit
	{
		public static void Seed(this ModelBuilder modelBuilder)
		{
			//modelBuilder.Entity<Category>().HasData(
			//	new Category {Id = 1, Name = "ABC", IsActive = true},
			//	new Category {Id = 2, Name = "EDF", IsActive = true}
			//);

		}
	}
}
