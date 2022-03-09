using Microsoft.EntityFrameworkCore;
using OngProject.Entities;
using System;

namespace OngProject.DataAccess
{
    public class SeedSlide
    {
        public void SeedSlides(ModelBuilder modelBuilder)
        {
            for (int i = 1; i < 11; i++)
            {
                modelBuilder.Entity<Slides>().HasData(
                    new Slides
                    {
                        Id = i,                        
                        ImageUrl = "Image " + i,
                        Text = "Text " + i,
                        Order = i,
                        OrganizationId=i,
                        SoftDelete = false,
                        LastModified = DateTime.Now
                    }
                );
            }
        }
    }
}