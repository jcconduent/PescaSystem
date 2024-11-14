using PescaSystem.Data;
using PescaSystem.Models;
using MongoDB.Driver;

namespace PescaSystem.Services
{
    public class UsuarioService
    {
        private readonly MongoDbContext _context;

        public UsuarioService(MongoDbContext context)
        {
            _context = context;
        }

        public Usuario ValidarUsuario(string nombreUsuario, string password)
        {
            return _context.Usuarios.Find(u => u.NombreUsuario == nombreUsuario && u.Password == password).FirstOrDefault();
        }
    }
}

