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
        public List<MessagesModel> GetMessages(string emisor, string receptor)
        {
            var listOne=_messages.Find<MessagesModel>(MessagesModel => MessagesModel.EmisorMsg == emisor).ToList();
            var listTwo = _messages.Find<MessagesModel>(MessagesModel => MessagesModel.EmisorMsg == receptor).ToList();

            var retornable = new List<MessagesModel>();
            foreach (var item in listOne)
            {
                if (item.ReceptorMsg==receptor)
                {
                    retornable.Add(item);
                }
            }
            foreach (var item in listTwo)
            {
                if (item.ReceptorMsg == emisor)
                {
                    retornable.Add(item);
                }
            }
            return retornable;
        }
        //Insertar nuevo mensaje a la base de datos
        public void CreateMessage(MessagesModel Mensaje, byte[] source, string fileName)
        {
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
        public List<MessagesModel> GetMessagesParam(string emisor, string receptor, string PalabraClave)
        {
            
            var listaMensajes = GetMessages(emisor,receptor);
            var listaParametro = new List<MessagesModel>();
            foreach (var item in listaMensajes)
            {
                if (item.Mensaje.Contains(PalabraClave))
                {
                    listaParametro.Add(item);
                }
            }
           
            return listaParametro;
        }
        //Obtiene mensaje especifico
        public MessagesModel GetMessage(string id)
        {
           return _messages.Find<MessagesModel>(MessagesModel => MessagesModel.Id == id).FirstOrDefault();
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
    }
}
