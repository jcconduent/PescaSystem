using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PescaSystem.Models
{
    public class PescaLog
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public DateTime Dia { get; set; }
        public required string Barco { get; set; }
        public required string Capitan { get; set; }
        public required string Cliente { get; set; }
        public double DieselConsumido { get; set; }
        public double MillasRecorridas { get; set; }
        public int SailfishRaises { get; set; }
        public int SailfishBites { get; set; }
        public int SailfishReleases { get; set; }
        public int MarlinRaises { get; set; }
        public int MarlinBites { get; set; }
        public int MarlinReleases { get; set; }
        public int DoradoRaises { get; set; }
        public int DoradoBites { get; set; }
        public int DoradoReleases { get; set; }
        public int TunaRaises { get; set; }
        public int TunaBites { get; set; }
        public int TunaReleases { get; set; }
        public string? Comentario { get; set; }
    }
}
