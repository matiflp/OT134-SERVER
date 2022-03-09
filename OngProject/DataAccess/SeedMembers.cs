using Microsoft.EntityFrameworkCore;
using OngProject.Entities;
using System;

namespace OngProject.DataAccess
{
    public class SeedMember
    {
        public void SeedMembers(ModelBuilder modelBuilder)
        {
            for (int i = 1; i < 11; i++)
            {
                modelBuilder.Entity<Member>().HasData(
                    new Member
                    {
                        Id = i,
                        Name = "Name " + i,
                        Image = "Image " + i,
                        FacebookUrl = "Facebook Url " + i,
                        InstagramUrl = "Instagram Url " + i,
                        LinkedinUrl = "Linkedin Url " + i,
                        Description = "Description " + i,
                        SoftDelete = false,
                        LastModified = DateTime.Now
                    }
                );
            }
        }
    }
}