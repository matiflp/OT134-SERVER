using OngProject.Entities;
using System.Collections.Generic;

namespace OngProject.Core.Interfaces
{
    public interface IRolesService
    {
        public IEnumerable<Rol> GetAll();
        public Rol GetById();
        public void Insert(Rol rol);
        public void Update(Rol rol);
        public void Delete(Rol rol);
    }
}