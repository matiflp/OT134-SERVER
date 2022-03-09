using Microsoft.EntityFrameworkCore;
using OngProject.Entities;
using System;

namespace OngProject.DataAccess
{
    public class SeedContact
    {
        public void SeedContacts(ModelBuilder modelBuilder)
        {
            for (int i = 1; i < 11; i++)
            {
                modelBuilder.Entity<Contacts>().HasData(
                    new Contacts
                    {
                        Id = i,
                        Name = "Contact Name " + i,
                        Phone = "Phone " + i,
                        Email = "name"+i+"@mail.com",
                        Message = "Message " + i,
                        SoftDelete = false,
                        LastModified = DateTime.Now
                    }
                );
            }
        }
    }
}