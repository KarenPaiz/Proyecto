using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using API.Models;
using API.Services;
using MongoDB.Driver.GridFS;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly ChatDatabaseService _chatDatabaseService;

        public ChatController(ChatDatabaseService chatDatabaseService, IChatDatabaseSettings settings)
        {
            _chatDatabaseService = chatDatabaseService;
        }
        //Obtener Todos Los Usuarios
        [Route("GetUsers")]
        public ActionResult<List<UsersModels>> GetUsers()
        {
            return Ok(_chatDatabaseService.GetAllUsers());
        }

        [Route("GetMsgs")]
        public ActionResult<List<MessagesModel>> GetMsgs(string[]Users)
        {
            var listaMensajes = _chatDatabaseService.GetMessages(Users[0],Users[1]);
            foreach (var item in listaMensajes)
            {
                var emisor = item.EmisorMsg;
                var validatedSend = _chatDatabaseService.ValidateUser(emisor);
                var receptor = item.ReceptorMsg;
                var validatedGet = _chatDatabaseService.ValidateUser(receptor);
                var secretKey = Libreria.Metodos.DiffieHelmannAlgorithm(validatedSend.IDDH, validatedGet.IDDH);
                var mensajeEncriptado = new byte[item.Mensaje.Length];
                for (int i = 0; i < item.Mensaje.Length; i++)
                {
                    mensajeEncriptado[i] = Convert.ToByte(Convert.ToChar(item.Mensaje[i]));
                }
                var messageEncriptado = Libreria.Metodos.DecryptZZ(mensajeEncriptado, Convert.ToInt32(secretKey));
                var mensajeGuardar = string.Empty;
                for (int i = 0; i < messageEncriptado.Length; i++)
                {
                    mensajeGuardar+= Convert.ToChar(messageEncriptado[i]);
                }
                item.Mensaje = mensajeGuardar;
            }
            return Ok(listaMensajes);
        }

        [Route("GetMsgsParam")]
        public ActionResult<List<MessagesModel>> GetMsgsParam(string[] parameters)
        {
            var listaMensajes = _chatDatabaseService.GetMessagesParam(parameters[0],parameters[1],parameters[2]);
            return Ok(listaMensajes);
        }

        [Route("GetFile")]
        public ActionResult<List<byte>> GetFile([FromBody]string FileName)
        {
            List<byte> archivo = _chatDatabaseService.GetFile(FileName);
            return Ok(archivo);
        }

        [Route("CreateUser")]
        public ActionResult<UsersModels> PostNewUser(UsersModels User)
        {
            var numeroDH = User.IDDH;
            var numeroCifrar = User.IDDH % User.Password.Length;
            var contraBytes = new byte[User.Password.Length];
            for (int i = 0; i < User.Password.Length; i++)
            {
                contraBytes[i] = Convert.ToByte(User.Password[i]);
            }
            var contrasenia = Libreria.Metodos.EncryptionZigZag(contraBytes, numeroCifrar);
            var passEncriptado = string.Empty;
            for (int i = 0; i < contrasenia.Length; i++)
            {
                passEncriptado+= Convert.ToChar(contrasenia[i]);
            }
            User.Password = passEncriptado;
            var creadoConExito = _chatDatabaseService.CreateUserSuccess(User);
            if (creadoConExito == false)
            {
                return Conflict();
            }
            return Ok();
        }

        [Route("SendMessage")]
        public ActionResult<UsersModels> PostMsg(MessagesModel Message)
        {
            var emisor = Message.EmisorMsg;
            var validatedSend = _chatDatabaseService.ValidateUser(emisor);
            var receptor = Message.ReceptorMsg;
            var validatedGet = _chatDatabaseService.ValidateUser(receptor);
            var secretKey = Libreria.Metodos.DiffieHelmannAlgorithm(validatedSend.IDDH, validatedGet.IDDH);
            var mensajeOriginal =new byte[Message.Mensaje.Length];
            for (int i = 0; i < Message.Mensaje.Length; i++)
            {
                mensajeOriginal[i]=Convert.ToByte(Convert.ToChar(Message.Mensaje[i]));
            }
            var messageEncriptado = Libreria.Metodos.EncryptionZigZag(mensajeOriginal, Convert.ToInt32(secretKey));
            var mensajeGuardar= string.Empty;
            for (int i = 0; i < messageEncriptado.Length; i++)
            {
                mensajeGuardar+= Convert.ToChar(messageEncriptado[i]);
            }
            Message.Mensaje = mensajeGuardar.ToString();
            var start = DateTime.Now;
            Message.FechaEnvio = start;
            _chatDatabaseService.CreateMessage(Message,Message.Archivo , Message.NombreArchivo);
            return Ok();
        }

        [Route("Login")]
        public ActionResult<bool> Login(string [] UsuarioPassword)
        {
            var validateduser = _chatDatabaseService.ValidateUser(UsuarioPassword[0]);
            var numeroCifrar = validateduser.IDDH % UsuarioPassword[1].Length;
            var contraBytes = new byte[validateduser.Password.Length];
            for (int i = 0; i < validateduser.Password.Length; i++)
            {
                contraBytes[i] = Convert.ToByte(validateduser.Password[i]);
            }
            var contrasenia = Libreria.Metodos.DecryptZZ(contraBytes,numeroCifrar);
            var contraseniaString = string.Empty;
            for (int i = 0; i < contrasenia.Length; i++)
            {
                contraseniaString += Convert.ToChar(contrasenia[i]);
            }
            if (contraseniaString==UsuarioPassword[1])
            {
                return true;
            }
            return false;
        }
        [Route("ValidateUser")]
        public ActionResult<bool> ValidateUser([FromBody]string usuarioBusqueda)
        {
            var UserDB = _chatDatabaseService.ValidateUser(usuarioBusqueda);
            var x = (UserDB != null) ? true : false;
            return Ok(x);
        }
    }
}
