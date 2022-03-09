using Microsoft.EntityFrameworkCore;
using OngProject.Entities;
using System;

namespace OngProject.DataAccess
{
    public class SeedComment
    {
        public void SeedComments(ModelBuilder modelBuilder)
        {
            for (int i = 1; i < 11; i++)
            {
                modelBuilder.Entity<Comment>().HasData(
                    new Comment
                    {
                        Id = i,
                        NewId = i,
                        UserId = i,
                        Body = "Body of the comment " + i,
                        SoftDelete = false,
                        LastModified = DateTime.Now
                    }
                );
            }
        }
    }
}