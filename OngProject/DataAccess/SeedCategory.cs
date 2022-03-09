using Microsoft.EntityFrameworkCore;
using OngProject.Entities;
using System;

namespace OngProject.DataAccess
{
    public class SeedCategory
    {
		public void SeedCategories(ModelBuilder modelBuilder)
		{
			for (int i = 1; i < 11; i++)
			{
				modelBuilder.Entity<Category>().HasData(
					new Category
					{
						Id = i,
						Name = "Category " + i,
						Description = "Description for Category" + i,
						Image = "image_category" + i,
						LastModified = DateTime.Now
					}
				);
			}
		}
	}
}
