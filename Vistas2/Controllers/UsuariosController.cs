﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using Vistas2.Models;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Net.Http.Formatting;
using System.Web.Script.Serialization;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace Vistas2.Controllers
{
    public class UsuariosController : Controller
    {
        // GET: Usuarios
        static string usuarioEnControl;
        static string usuarioReceptor;
        public static string key = "MauricioYSamanthaGanaranEsteCurso";
        public static string token;
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IngresoU()
        {
            return View();
        }

        [HttpPost]
        public ActionResult IngresoU(HttpPostedFileBase collection, string Usuario, string Nombre, string Password)
        {
            if (Usuario == null && Password == null)
            {
                return Json(new { status = "error", message = "error al ingresar datos" });
            }
            else
            {
                var nombreUsuario = Nombre;
                var usuario = Usuario;
                var contrasenia = string.Empty;
                Random rnd = new Random();
                int aNumber = rnd.Next(1, 100);
                List<int> numbers = new List<int>();
                if (System.IO.File.Exists(Server.MapPath(@"~\Numbers.txt")))
                {
                    using (var lectura = new StreamReader(Server.MapPath(@"~\Numbers.txt")))
                    {
                        string linea;
                        while ((linea = lectura.ReadLine()) != null)
                        {
                            numbers.Add(Convert.ToInt16(lectura.ReadLine()));
                        }
                    }
                    bool existe = false;
                    while (!existe)
                    {
                        existe = true;
                        foreach (var item in numbers)
                        {
                            if (item == aNumber)
                            {
                                existe = false;
                            }
                        }
                        if (!existe)
                        {
                            aNumber = rnd.Next(1, 100);
                        }
                    }
                    using (var escritura = System.IO.File.AppendText(Server.MapPath(@"~\Numbers.txt")))
                    {
                        escritura.WriteLine(aNumber);
                    }
                }
                else
                {
                    using (var escritura = new StreamWriter(Server.MapPath(@"~\Numbers.txt")))
                    {
                        escritura.WriteLine(aNumber);
                    }
                }
                var byteContrasenia = new List<byte>();
                foreach (var item in Password)
                {
                    byteContrasenia.Add(Convert.ToByte(Convert.ToChar(item)));
                }
                var numeroCifrar = Password.Length - 1 % aNumber;
                byte[] contrasenia1 = Libreria.Metodos.EncryptionZigZag(byteContrasenia.ToArray(), numeroCifrar);

                foreach (var item in contrasenia1)
                {
                    contrasenia += Convert.ToChar(item).ToString();
                }
                var usuarioNuevo = new UsersModels();
                usuarioNuevo.IDDH = aNumber;
                usuarioNuevo.Nombre = Nombre;
                usuarioNuevo.Password = contrasenia;
                usuarioNuevo.Usuario = Usuario;
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:59679");
                    HttpResponseMessage response = client.PostAsync("api/Chat/UserExists", Usuario, new JsonMediaTypeFormatter()).Result;
                    var postResult = response.Content.ReadAsStringAsync().Result;
                    JavaScriptSerializer JSserializer = new JavaScriptSerializer();
                    bool returnbool = JSserializer.Deserialize<bool>(postResult);
                    if (!returnbool)
                    {
                        HttpResponseMessage response2 = client.PostAsync("api/Chat/CreateUser", usuarioNuevo, new JsonMediaTypeFormatter()).Result;

                        if (response2.IsSuccessStatusCode)
                        {
                            return RedirectToAction("Inicio");
                        }
                    }

                    ModelState.AddModelError(string.Empty, "Error, usuario ya existente");
                }
                ModelState.AddModelError(string.Empty, "Error");
            }
            return View();
        }

        [HttpPost]
        public ActionResult Inicio(HttpPostedFileBase collection, string Usuario, string Password)
        {
            string[] UsuarioPassword = { Usuario, Password };
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:59679");
                HttpResponseMessage response = client.PostAsync("api/Chat/Login", UsuarioPassword, new JsonMediaTypeFormatter()).Result;
                if (response.IsSuccessStatusCode)
                {
                    var postResult = response.Content.ReadAsStringAsync().Result;
                    JavaScriptSerializer JSserializer = new JavaScriptSerializer();
                    bool returnbool = JSserializer.Deserialize<bool>(postResult);
                    if (returnbool)
                    {
                        var diccionario = new Dictionary<string, string>();
                        diccionario.Add("Usuario", Usuario);
                        usuarioEnControl = Usuario;
                        Response.Cookies["UsuariosApp"]["UsuarioControl"] = Usuario;
                        Response.Cookies["UsuariosApp"]["UsuarioReceptor"] = "";
                        token = generandoToken(Usuario);
                        Response.Cookies["UsuariosApp"].Expires = DateTime.Now.AddDays(5);
                        return RedirectToAction("SalaDeChat");
                    }
                    return RedirectToAction("Inicio");
                }
            }
            ModelState.AddModelError(string.Empty, "Error");
            return View();
        }

        public ActionResult Inicio()
        {

            return View();
        }

        [HttpPost]
        public ActionResult SalaDeChat(string usuarioBusqueda)
        {
            Response.Cookies["UsuariosApp"]["UsuarioControl"] = usuarioEnControl;
            if (usuarioBusqueda != null)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:59679");
                    HttpResponseMessage response = client.PostAsync("api/Chat/ValidateUser", usuarioBusqueda, new JsonMediaTypeFormatter()).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var postResult = response.Content.ReadAsStringAsync().Result;
                        JavaScriptSerializer JSserializer = new JavaScriptSerializer();
                        bool returnbool = JSserializer.Deserialize<bool>(postResult);
                        if (returnbool)
                        {
                            usuarioReceptor = usuarioBusqueda;
                            Response.Cookies["UsuariosApp"]["UsuarioReceptor"] = usuarioReceptor;

                            return RedirectToAction("Chats");
                        }
                        return RedirectToAction("SalaDeChat");
                    }
                }
            }
            return RedirectToAction("SalaDeChat");
        }

        public ActionResult SalaDeChat()
        {
            Response.Cookies["UsuariosApp"]["UsuarioControl"] = usuarioEnControl;
            usuarioReceptor = string.Empty;
            var usuarios = new List<UsersModels>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:59679");
                HttpResponseMessage postJob = client.GetAsync("api/Chat/GetUsers").Result;
                if (postJob.IsSuccessStatusCode)
                {
                    var postResult = postJob.Content.ReadAsStringAsync().Result;
                    JavaScriptSerializer JSserializer = new JavaScriptSerializer();
                    var returnList = JSserializer.Deserialize<List<UsersModels>>(postResult);
                    usuarios = returnList;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error");
                }
            }
            ViewBag.Matriz = usuarios.ToArray();
            return View();
        }

        [HttpPost]
        public ActionResult Chats(string Archivo)
        {
            if (!validandoToken(token))
            {
                return RedirectToAction("Inicio");
            }
            Response.Cookies["UsuariosApp"]["UsuarioReceptor"] = usuarioReceptor;
            Response.Cookies["UsuariosApp"]["UsuarioControl"] = usuarioEnControl;
            Directory.CreateDirectory("C:/App_Data/ArchivosDescargas/");
            if (Archivo != null)
            {
                byte[] aEscribir;
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:59679");
                    HttpResponseMessage response = client.PostAsync("api/Chat/GetFile", Archivo, new JsonMediaTypeFormatter()).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var readJob = response.Content.ReadAsAsync<byte[]>().Result;
                        JavaScriptSerializer JSserializer = new JavaScriptSerializer();
                        aEscribir = Libreria.Metodos.LZWDecompress(readJob);
                        using (var writestream = new FileStream("C:/App_Data/ArchivosDescargas/" + Archivo, FileMode.OpenOrCreate))
                        {
                            using (var writer = new BinaryWriter(writestream))
                            {
                                foreach (var item in aEscribir)
                                {
                                    writer.Write(item);
                                }
                            }
                        }
                        var fs = System.IO.File.OpenRead("C:/App_Data/ArchivosDescargas/" + Archivo);

                        return File(fs, "application/force-download", Archivo);
                    }
                }

                //Escribir a archivo
            }
            var mensajs = new List<MessagesModel>();
            using (var client = new HttpClient())
            {
                string[] usuariosActivos = { usuarioEnControl, usuarioReceptor };
                client.BaseAddress = new Uri("http://localhost:59679");
                HttpResponseMessage response = client.PostAsync("api/Chat/Msgs", usuariosActivos, new JsonMediaTypeFormatter()).Result;
                if (response.IsSuccessStatusCode)
                {
                    var readJob = response.Content.ReadAsAsync<List<MessagesModel>>();
                    mensajs = readJob.Result;
                }
            }
            ViewBag.Matriz = mensajs.ToArray();
            return View();
        }

        public ActionResult Chats()
        {
            if (!validandoToken(token))
            {
                return RedirectToAction("Inicio");
            }
            Response.Cookies["UsuariosApp"]["UsuarioReceptor"] = usuarioReceptor;
            Response.Cookies["UsuariosApp"]["UsuarioControl"] = usuarioEnControl;
            var mensajs = new List<MessagesModel>();
            using (var client = new HttpClient())
            {
                string[] usuariosActivos = { usuarioEnControl, usuarioReceptor };
                client.BaseAddress = new Uri("http://localhost:59679");
                HttpResponseMessage response = client.PostAsync("api/Chat/GetMsgs", usuariosActivos, new JsonMediaTypeFormatter()).Result;
                if (response.IsSuccessStatusCode)
                {
                    var readJob = response.Content.ReadAsAsync<List<MessagesModel>>();
                    mensajs = readJob.Result;
                }
            }
            ViewBag.Matriz = mensajs.ToArray();

            return View();
        }

        public ActionResult EnvioMensajes(HttpPostedFileBase ArchivoImportado, string Mensaje)
        {
            Response.Cookies["UsuariosApp"]["UsuarioReceptor"] = usuarioReceptor;
            Response.Cookies["UsuariosApp"]["UsuarioControl"] = usuarioEnControl;
            var ObjetoMensaje = new MessagesModel();
            if (Mensaje != null && ArchivoImportado != null)
            {
                ObjetoMensaje.Mensaje = Mensaje;
                ObjetoMensaje.FechaEnvio = DateTime.Now;
                ObjetoMensaje.EmisorMsg = usuarioEnControl;
                ObjetoMensaje.ReceptorMsg = usuarioReceptor;
                var bytesArchivo = new List<byte>();
                using (var Lectura = new BinaryReader(ArchivoImportado.InputStream))
                {
                    while (Lectura.BaseStream.Position != Lectura.BaseStream.Length)
                    {
                        bytesArchivo.Add(Lectura.ReadByte());
                    }
                }
                ObjetoMensaje.Archivo = Libreria.Metodos.LZWCompress(bytesArchivo.ToArray(), ArchivoImportado.FileName);
                ObjetoMensaje.NombreArchivo = ArchivoImportado.FileName;
                ObjetoMensaje.PoseeArchivo = true;
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:59679");
                    var postJob = client.PostAsync("api/Chat/SendMessage", ObjetoMensaje, new JsonMediaTypeFormatter()).Result;
                    if (postJob.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Chats");
                    }
                }
            }
            else if (ArchivoImportado != null && Mensaje == null)
            {
                ObjetoMensaje.Mensaje = " ";
                ObjetoMensaje.FechaEnvio = DateTime.Now;
                ObjetoMensaje.EmisorMsg = usuarioEnControl;
                ObjetoMensaje.ReceptorMsg = usuarioReceptor;
                var bytesArchivo = new List<byte>();
                using (var Lectura = new BinaryReader(ArchivoImportado.InputStream))
                {
                    while (Lectura.BaseStream.Position != Lectura.BaseStream.Length)
                    {
                        bytesArchivo.Add(Lectura.ReadByte());
                    }
                }
                ObjetoMensaje.Archivo = Libreria.Metodos.LZWCompress(bytesArchivo.ToArray(), ArchivoImportado.FileName);
                ObjetoMensaje.NombreArchivo = ArchivoImportado.FileName;
                ObjetoMensaje.PoseeArchivo = true;
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:59679");
                    var postJob = client.PostAsync("api/Chat/SendMessage", ObjetoMensaje, new JsonMediaTypeFormatter()).Result;

                    if (postJob.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Chats");
                    }
                }
            }
            else if (ArchivoImportado == null && Mensaje != null)
            {
                ObjetoMensaje.Mensaje = Mensaje;
                ObjetoMensaje.FechaEnvio = DateTime.Now;
                ObjetoMensaje.EmisorMsg = usuarioEnControl;
                ObjetoMensaje.ReceptorMsg = usuarioReceptor;
                ObjetoMensaje.PoseeArchivo = false;
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:59679");
                    var postJob = client.PostAsync("api/Chat/SendMessage", ObjetoMensaje, new JsonMediaTypeFormatter()).Result;
                    if (postJob.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Chats");
                    }
                }
            }

            return View();
        }

        [HttpPost]
        public ActionResult BusquedaM(string Mensaje)
        {
            Response.Cookies["UsuariosApp"]["UsuarioReceptor"] = usuarioReceptor;
            Response.Cookies["UsuariosApp"]["UsuarioControl"] = usuarioEnControl;
            string[] parameters = { usuarioEnControl, usuarioReceptor, Mensaje };

            var mensajs = new List<MessagesModel>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:59679");
                var postJob = client.PostAsync("api/Chat/GetMsgsParam", parameters, new JsonMediaTypeFormatter()).Result;
                if (postJob.IsSuccessStatusCode)
                {
                    var readJob = postJob.Content.ReadAsAsync<List<MessagesModel>>();
                    readJob.Wait();
                    mensajs = readJob.Result;
                }
            }
            ViewBag.Matriz = mensajs.ToArray();

            return View();
        }
        public ActionResult BusquedaM()
        {
            Response.Cookies["UsuariosApp"]["UsuarioReceptor"] = usuarioReceptor;
            Response.Cookies["UsuariosApp"]["UsuarioControl"] = usuarioEnControl;
            var mensajs = new List<MessagesModel>();

            using (var client = new HttpClient())
            {
                string[] usuariosActivos = { usuarioEnControl, usuarioReceptor };
                client.BaseAddress = new Uri("http://localhost:59679");
                var postJob = client.PostAsync("api/Chat/GetMsgs", usuariosActivos, new JsonMediaTypeFormatter()).Result;

                if (postJob.IsSuccessStatusCode)
                {
                    var readJob = postJob.Content.ReadAsAsync<List<MessagesModel>>();
                    readJob.Wait();
                    mensajs = readJob.Result;
                }
            }
            ViewBag.Matriz = mensajs.ToArray();
            return View();
        }
        public ActionResult LogOut()
        {
            usuarioEnControl = " ";
            usuarioReceptor = " ";
            return RedirectToAction("Inicio");
        }

        public ActionResult DescargarArchivos(string nombre)
        {
            Response.Cookies["UsuariosApp"]["UsuarioReceptor"] = usuarioReceptor;
            Response.Cookies["UsuariosApp"]["UsuarioControl"] = usuarioEnControl;
            var nombreArchivo = nombre;


            return View();
        }
        public string generandoToken(string objeto)
        {
            var buffer = key.PadRight(64, ' ')
              .ToCharArray()
              .Select(x => Convert.ToByte(x))
              .ToArray();
            var handler = new JwtSecurityTokenHandler();
            var claims = objeto;


            var description = new SecurityTokenDescriptor
            {
                Issuer = "todos",
                Audience = "audiencia",
                Expires = DateTime.UtcNow.AddMinutes(5),
                Subject = new ClaimsIdentity(claims),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(buffer), SecurityAlgorithms.HmacSha256)
            };

            var token = handler.CreateToken(description);
            var tokenstring = handler.WriteToken(token);
            return tokenstring;
        }
        public bool validandoToken(string tokenAValidar)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(tokenAValidar);
            var comprueba = true;
            DateTime tokenDate = token.ValidTo;
            if (tokenDate<DateTime.Now)
            {
                comprueba = false;
            }
            return comprueba;
        }
    }
}