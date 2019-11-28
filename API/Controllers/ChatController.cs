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
        public ActionResult<List<MessagesModel>> GetMsgs(string UserOne, string UserTwo)
        {
            var listaMensajes = _chatDatabaseService.GetMessages(UserOne, UserTwo);
            return Ok(listaMensajes);
        }

        [Route("GetMsgsParam")]
        public ActionResult<List<MessagesModel>> GetMsgsParam(string UsuarioUno, string UsuarioDos, string Parameter)
        {
            var listaMensajes = _chatDatabaseService.GetMessagesParam(UsuarioUno, UsuarioDos, Parameter);
            return Ok(listaMensajes);
        }

        [Route("GetFile")]
        public ActionResult<List<byte>> GetFile(string FileName)
        {
            List<byte> archivo = _chatDatabaseService.GetFile(FileName);
            return Ok(archivo);
        }

        [Route("CreateUser")]
        public ActionResult<UsersModels> PostNewUser(UsersModels User)
        {
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
            var start = DateTime.Now;
            Message.FechaEnvio = start;
            _chatDatabaseService.CreateMessage(Message,Message.Archivo , Message.NombreArchivo);
            return Ok();
        }

        [Route("Login")]
        public ActionResult<bool> Login(string Usuario, string Password)
        {
            var UserDB = _chatDatabaseService.GetUser(Usuario, Password);
            var x = (UserDB != null) ? true : false;
            return Ok(x);
        }
    }
}
