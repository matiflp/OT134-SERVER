using Microsoft.EntityFrameworkCore;
using OngProject.Entities;
using System;

namespace OngProject.DataAccess
{
    public class SeedTestimonial
    {
        public void SeedTestimonials(ModelBuilder modelBuilder)
        {
            for (int i = 1; i < 11; i++)
            {
                modelBuilder.Entity<Testimonials>().HasData(
                    new Testimonials
                    {
                        Id = i,
                        Name = "Name testimonial " + i,
                        Content = "Content testimonial" + i,
                        Image = "Image testimonial " + i,
                        SoftDelete = false,
                        LastModified = DateTime.Now
                    }
                );
            }
        }
    }
}