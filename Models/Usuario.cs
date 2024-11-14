using MongoDB.Bson;

namespace PescaSystem.Models
{
    public class Usuario
    {
        public ObjectId Id { get; set; }
        public required string NombreUsuario { get; set; }
        public required string Password { get; set; }
    }
}
