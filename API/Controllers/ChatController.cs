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
            return _chatDatabaseService.GetAllUsers();
        }

        [Route("GetMsgs")]
        public ActionResult<List<MessagesModel>> GetMsgs(string Emisor, string Receptor)
        {
            var listaMensajes = _chatDatabaseService.GetMessages(Emisor, Receptor);
            return listaMensajes;
        }

        [Route("GetMsgParam")]
        public ActionResult<List<MessagesModel>> GetMsgParam(string Emisor, string Receptor, string Parameter)
        {
            var listaMensajes = _chatDatabaseService.GetMessagesParam(Emisor, Receptor, Parameter);
            return listaMensajes;
        }

        [Route("GetFile")]
        public List<byte> GetFile(string FileName)
        {
            List<byte> archivo = _chatDatabaseService.GetFile(FileName);
            return archivo;
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
            var source = new byte[5]; source[0] = 34; source[1] = 202; source[2] = 104; source[3] = 37; source[4] = 23;
            var fileName = "LaJuana.txt";
            _chatDatabaseService.CreateMessage(Message, source, fileName);
            return Ok();
        }

        [Route("Login")]
        public bool Login(string Usuario, string Password)
        {
            var UserDB = _chatDatabaseService.GetUser(Usuario, Password);
            var x = (UserDB != null) ? true : false;
            return x;
        }
    }
}