using Microsoft.EntityFrameworkCore;
using OngProject.Entities;

namespace OngProject.DataAccess
{
    public class SeedUserRol
    {
        public void SeedRoles(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Rol>().HasData(
                new Rol
                {
                    Id = 1,
                    Name = "User",
                    Description = "Regular user without permissions"
                },
                new Rol
                {
                    Id = 2,
                    Name = "Administrator",
                    Description = "Administrator user with permissions"
                }
            );

        }
    }
}
