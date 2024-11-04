using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PescaSystem.Models
{
    public class PescaLog
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public required string Fecha { get; set; }
        public required string Yate { get; set; } 
        public required string Capitan { get; set; } 
        public required string Grupo { get; set; }
        public double DieselConsumido { get; set; }
        public double MillasRecorridas { get; set; }
        public int VelaFlotados { get; set; }
        public int VelaPiques { get; set; }
        public int VelaLiberados { get; set; }
        public int VelaMoscaFlotados { get; set; }
        public int VelaMoscaPiques { get; set; }
        public int VelaMoscaLiberados { get; set; }
        public int MarlinAzulFlotados { get; set; }
        public int MarlinAzulPiques { get; set; }
        public int MarlinAzulLiberados { get; set; }
        public int MarlinMoscaAzulFlotados { get; set; }
        public int MarlinMoscaAzulPiques { get; set; }
        public int MarlinMoscaAzulLiberados { get; set; }
        public int MarlinRayadoFlotados { get; set; }
        public int MarlinRayadoPiques { get; set; }
        public int MarlinRayadoLiberados { get; set; }
        public int MarlinMoscaRayadoFlotados { get; set; }
        public int MarlinMoscaRayadoPiques { get; set; }
        public int MarlinMoscaRayadoLiberados { get; set; }
        public int MarlinNegroFlotados { get; set; }
        public int MarlinNegroPiques { get; set; }
        public int MarlinNegroLiberados { get; set; }
        public int Dorado { get; set; }
        public int DoradoFly { get; set; }
        public int Atun { get; set; }
        public int AtunFly { get; set; }
        public int Gallos { get; set; }
        public int Wahoo { get; set; }
        public string? Comentario { get; set; }
    }
}
