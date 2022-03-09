using Microsoft.EntityFrameworkCore;
using OngProject.Entities;
using System;

namespace OngProject.DataAccess
{
    public class SeedNew
    {
        public void SeedNews(ModelBuilder modelBuilder)
        {
            for (int i = 1; i < 11; i++)
            {
                modelBuilder.Entity<New>().HasData(
                    new New
                    {
                        Id = i,
                        Name = "New " + i,
                        Content = "Content for New " + i,
                        Image = "image_new " + i,
                        LastModified = DateTime.Now,
                        CategoryId = i,
                    }
                );
            }
        }
    }
}