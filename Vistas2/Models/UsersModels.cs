using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vistas2.Models
{
    public class UsersModels
    {
        //[BsonId]
        //[BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string Usuario { get; set; }
        public string Password { get; set; }
        public int IDDH { get; set; }
    }
}