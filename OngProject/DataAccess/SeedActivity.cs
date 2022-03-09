using Microsoft.EntityFrameworkCore;
using OngProject.Entities;
using System;

namespace OngProject.DataAccess
{
    public class SeedActivity
    {
		public void SeedActivities(ModelBuilder modelBuilder)
		{
			for (int i = 1; i < 11; i++)
			{
				modelBuilder.Entity<Activities>().HasData(
					new Activities
					{
						Id = i,
						Name = "Activity " + i,
						Content = "Content from activity " + i,
						Image = "Image from activity " + i,
						LastModified = DateTime.Now
					}
				);
			}
		}
	}
}
