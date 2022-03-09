using Microsoft.EntityFrameworkCore;
using OngProject.Core.Helper;
using OngProject.DataAccess;
using OngProject.Entities;
using System;

namespace Test.Helper
{
    internal class DbContextHelper
    {
        private static AppDbContext _context;

        public static AppDbContext MakeDbContext(bool pupulate = true)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>().
                UseInMemoryDatabase(databaseName: "Ong").
                Options;
           
            _context = new AppDbContext(options);
           
            _context.Database.EnsureDeleted();

            if (pupulate)
            {
                SeedRoles();
                SeedUsers();
                SeedMembers();
                SeedOrganization();
                SeedSlide();
                SeedActivities();
                SeedNews();
                SeedCategory();
                SeedContacts();
                SeedTestimonials();
                _context.SaveChanges();
            }

            return _context;
        }
        
        private static void SeedRoles()
        {
            _context.Add(new Rol
            {
                Id = 1,
                Name = "User",
                Description = "Regular user without permissions"
            });
            _context.Add(new Rol
            {
                Id = 2,
                Name = "Administrator",
                Description = "Administrator user with permissions"
            });
        }

        private static void SeedUsers()
        {
            for (int i = 1; i < 21; i++)
            {
                _context.Add(
                    new User
                    {
                        Id = i,
                        FirstName = "Name User " + i,
                        LastName = "Last Name User" + i,
                        Email = "User" + i + "@ong.com",
                        Password = EncryptHelper.GetSHA256("Password" + i),
                        Photo = "Photo" + i,
                        SoftDelete = i == 1,
                        RolId = i < 11 ? 1 : 2,
                        LastModified = DateTime.Now
                    }
                );
            }
        }

        private static void SeedMembers()
        {
            //agrego el miembro 1 con un Soft Delete = true para verificar que no se puede eliminar un miembro ya eliminado
            for (int i = 1; i < 12; i++)
            {
                _context.Add(
                    new Member
                    {
                        Id = i,
                        Name = "Name " + i,
                        Image = "Image " + i,
                        FacebookUrl = "Facebook Url " + i,
                        InstagramUrl = "Instagram Url " + i,
                        LinkedinUrl = "Linkedin Url " + i,
                        Description = "Description " + i,
                        SoftDelete = i == 1,
                        LastModified = DateTime.Now
                    }
                );
            }
        }

        private static void SeedOrganization()
        {
            for (int i = 1; i < 11; i++)
            {
                _context.Add(
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

        public static void SeedSlide()
        {
            for (int i = 1; i < 11; i++)
            {
                _context.Add(
                    new Slides
                    {
                        Id = i,
                        ImageUrl = "Image " + i,
                        Text = "Text " + i,
                        Order = i,
                        OrganizationId = i,
                        SoftDelete = false,
                        LastModified = DateTime.Now
                    }
               );
            }
        }

        private static void SeedActivities()
        {
            for (int i = 1; i < 11; i++)
            {
                _context.Add(
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

        private static void SeedNews()
        {
            for (int i = 1; i < 11; i++)
            {
                _context.Add(
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
      
        private static void SeedCategory()
        {
            for (int i = 1; i < 11; i++)
            {
                _context.Add(
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

        public static void SeedContacts()
        {
            for (int i = 1; i < 11; i++)
            {
                _context.Add(
                    new Contacts
                    {
                        Id = i,
                        Name = "Contact Name " + i,
                        Phone = "Phone " + i,
                        Email = "name" + i + "@mail.com",
                        Message = "Message " + i,
                        SoftDelete = false,
                        LastModified = DateTime.Now
                    }
                );
            }
        }

        public static void SeedTestimonials()
        {
            for (int i = 1; i < 11; i++)
            {
                _context.Add(
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
