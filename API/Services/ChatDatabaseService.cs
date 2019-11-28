using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using API.Models;
using MongoDB.Bson;
using MongoDB.Driver.GridFS;
using System.IO;

namespace API.Services
{
    public class ChatDatabaseService
    {
        private readonly IMongoCollection<UsersModels> _users;
        private readonly IMongoCollection<MessagesModel> _messages;
        private readonly string dbName;
        private readonly GridFSBucket gridFsBucket;        
        public ChatDatabaseService(IChatDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            dbName = settings.DatabaseName;
            gridFsBucket = new GridFSBucket(database);
            _users = database.GetCollection<UsersModels>(settings.UsersCollectionName);
            _messages = database.GetCollection<MessagesModel>(settings.MessagesCollectionName);
        }
        //Obtener la lista de usuarios disponibles
        public List<UsersModels> GetAllUsers() =>
           _users.Find(UsersModels => true).ToList();
        //Crear nuevo usuario
        public bool CreateUserSuccess(UsersModels Usuario)
        {
            var listaUsers = _users.Find(UsersModels => true).ToList();
            foreach (var item in listaUsers)
            {
                if (item.Usuario == Usuario.Usuario)
                {
                    return false;
                }
            }
            _users.InsertOne(Usuario);
            return true;
        }
        //Obtener los mensajes con parametros de receptor y emisor
        public List<MessagesModel> GetMessages(string UserOne, string UserTwo)
        {
            var listOne=_messages.Find<MessagesModel>(MessagesModel => MessagesModel.EmisorMsg == UserOne).ToList();
            var listTwo = _messages.Find<MessagesModel>(MessagesModel => MessagesModel.EmisorMsg == UserTwo).ToList();

            var retornable = new List<MessagesModel>();
            foreach (var item in listOne)
            {
                if (item.ReceptorMsg==UserTwo)
                {
                    retornable.Add(item);
                }
            }
            foreach (var item in listTwo)
            {
                if (item.ReceptorMsg == UserOne)
                {
                    retornable.Add(item);
                }
            }
            retornable.Sort((a, b) => b.FechaEnvio.CompareTo(a.FechaEnvio));
            return retornable;
        }
        //Insertar nuevo mensaje a la base de datos
        public void CreateMessage(MessagesModel Mensaje, byte[] source, string fileName)
        {
            Mensaje.Archivo = null;
            var NombreArchivo = fileName;
            if (Mensaje.PoseeArchivo)
            {
                int i = 0;
                while (true)
                {
                    var x = GetFile(NombreArchivo);
                    if (x == null)
                    {
                        AddFile(source, NombreArchivo);
                        Mensaje.NombreArchivo = NombreArchivo;
                        _messages.InsertOne(Mensaje);
                        break;
                    }
                    else
                    {
                        var propiedades = fileName.Split(".");
                        NombreArchivo = propiedades[0]+i+"."+propiedades[1];
                        i++;
                    }
                }              
                
            }
            else
            {
                _messages.InsertOne(Mensaje);
            }
        }
        //Agrega archivo
        public void AddFile(byte[] source, string fileName)
        {
            gridFsBucket.UploadFromBytes(fileName, source);
            
        }
        //Obtiene archivo
        public List<byte> GetFile(string fileName)
        {
            try
            {
                var archivo2 = gridFsBucket.DownloadAsBytesByName(fileName);
                var listBytesFile = archivo2.ToList();
                return listBytesFile;
            }
            catch (Exception)
            {
                return null;
            }
        }
        //Hace la busqueda con los parametros de emisor, receptor y palabra clave
        public List<MessagesModel> GetMessagesParam(string UserOne, string UserTwo, string PalabraClave)
        {
            
            var listaMensajes = GetMessages(UserOne,UserTwo);
            var listaParametro = new List<MessagesModel>();
            foreach (var item in listaMensajes)
            {
                if (item.Mensaje.Contains(PalabraClave))
                {
                    listaParametro.Add(item);
                }
            }
            listaParametro.Sort((a, b) => b.FechaEnvio.CompareTo(a.FechaEnvio));
            return listaParametro;
        }
        public UsersModels GetUser(string Usuario, string password)
        {
            var listaUsuarios= _users.Find(UsersModels => UsersModels.Usuario== Usuario).ToList();
            foreach (var item in listaUsuarios)
            {
                if (item.Password==password)
                {
                    return item;
                }
            }
            return null;
        }
        public UsersModels ValidateUser(string Usuario)
        {
            return _users.Find(UsersModels => UsersModels.Usuario == Usuario).FirstOrDefault();            
        }
    }
}
