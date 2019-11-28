using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace vistas.Models
{
    public class MessagesModel
    {
        //[BsonId]
        //[BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string EmisorMsg { get; set; }
        public string ReceptorMsg { get; set; }
        public string Mensaje { get; set; }
        //[BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime FechaEnvio { get; set; }
        public bool PoseeArchivo { get; set; }
        public string NombreArchivo { get; set; }
        public byte[] Archivo { get; set; }

    }
}
