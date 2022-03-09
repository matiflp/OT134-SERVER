using Microsoft.EntityFrameworkCore;
using OngProject.Entities;
using System;

namespace OngProject.DataAccess
{
    public class SeedOrganization
    {
        public void SeedOrganizations(ModelBuilder modelBuilder)
        {
            for (int i = 1; i < 11; i++)
            {
                modelBuilder.Entity<Organization>().HasData(
                    new Organization
                    {
                        Id = i,
                        Name = "Organization name " + i,
                        Image = "Image " + i,
                        Address = "Address " + i,
                        Phone = int.MaxValue,
                        Email = "name@organization.com",
                        WelcomeText = "Welcome text " + i,
                        AboutUsText = "About us text " + i,
                        FacebookUrl = "Facebook url " + i,
                        InstagramUrl = "Instagram url " + i,
                        LinkedinUrl = "Linkedin url " + i,
                        SoftDelete = false,
                        LastModified = DateTime.Now
                    }
                );
            }
        }
    }
}