using Microsoft.EntityFrameworkCore;
using OngProject.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OngProject.DataAccess
{
    public class SeedDataMember
    {
        public void SeedMembers(ModelBuilder modelBuilder) {
            for (int i = 1; i < 5; i++) {
                modelBuilder.Entity<Member>().HasData(
                    new Member
                    {
                        Id = i,
                        Name = "Name User" + i,
                        Description = "User description" + i,
                        Image = "User image" + i,
                        FacebookUrl = "Facebook url user" + i,
                        InstagramUrl = "Instagram url user" + i,
                        LinkedinUrl = "Linkedin url user" + i,
                        LastModified = DateTime.Now,
                        SoftDelete = false
                    }
                    );
            }
        }
    }
}
