using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using vistas.Models;
using System.Net.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Security.Principal;
using System.Text;

namespace vistas.Controllers
{
    public class UsuariosController : Controller
    {
        static string usuarioEnControl;
        static string usuarioReceptor;
        public static string key = "MauricioYSamanthaGanaranEsteCurso";
        public ActionResult IngresoU()
        {
            return View();
        }
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult IngresoU(IFormCollection collection, string Usuario, string Nombre, string Password)
        {
            var nombreUsuario = Nombre;
            var usuario = Usuario;
            var contrasenia = string.Empty;
            Random rnd = new Random();
            int aNumber = rnd.Next(1, 100);
            List<int> numbers = new List<int>();
            if (System.IO.File.Exists("Numbers.txt"))
            {
                using (var lectura = new StreamReader("Numbers.txt"))
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
                using (var escritura = System.IO.File.AppendText("Numbers.txt"))
                {
                    escritura.WriteLine(aNumber);
                }
            }
            else
            {
                using (var escritura = new StreamWriter("Numbers.txt"))
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

            //ENVIAR DATOS A MAURICIO
            //ENVIAR AL USUARIO A LA PAGINA DE INICIO
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:59679");
                var postJob = client.PostAsJsonAsync<UsersModels>("api/Chat/CreateUser", usuarioNuevo);
                postJob.Wait();
                var postResult = postJob.Result;
                if (postResult.IsSuccessStatusCode)
                {
                    return RedirectToAction("Inicio");
                }
            }
            ModelState.AddModelError(string.Empty, "Error");
            return View();
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Inicio(IFormCollection collection, string Usuario, string Password)
        {
            string[] UsuarioPassword = { Usuario, Password };
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:59679");
                var postJob = client.PostAsJsonAsync<string[]>("api/Chat/Login", UsuarioPassword);
                postJob.Wait();
                var postResult = postJob.Result;
                if (postResult.IsSuccessStatusCode)
                {
                    var readJob = postResult.Content.ReadAsAsync<bool>();
                    readJob.Wait();
                    if (readJob.Result)
                    {
                        var diccionario = new Dictionary<string, string>();
                        diccionario.Add("Usuario", Usuario);
                        usuarioEnControl = Usuario;
                        cookieUsuarios("UsuarioControl", Usuario);
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
            usuarioEnControl = obtainCookieUsuarios("UsuarioControl");
            if (usuarioBusqueda != null)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:59679");
                    var postJob = client.PostAsJsonAsync("api/Chat/ValidateUser", usuarioBusqueda);
                    postJob.Wait();
                    var postResult = postJob.Result;
                    if (postResult.IsSuccessStatusCode)
                    {
                        var readJob = postResult.Content.ReadAsAsync<bool>();
                        readJob.Wait();
                        if (readJob.Result)
                        {
                            cookieUsuarios("UsuarioReceptor", usuarioBusqueda);
                            usuarioReceptor = usuarioBusqueda;

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
            usuarioEnControl = obtainCookieUsuarios("UsuarioControl");
            usuarioReceptor = string.Empty;
            var usuarios = new List<UsersModels>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:59679");
                var postJob = client.GetAsync("api/Chat/GetUsers");
                postJob.Wait();
                var postResult = postJob.Result;
                if (postResult.IsSuccessStatusCode)
                {
                    var readJob = postResult.Content.ReadAsAsync<List<UsersModels>>();
                    readJob.Wait();
                    usuarios = readJob.Result;
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
            usuarioReceptor = obtainCookieUsuarios("UsuarioReceptor");
            usuarioEnControl = obtainCookieUsuarios("UsuarioControl");
            Directory.CreateDirectory("C:/App_Data/ArchivosDescargas/");
            if (Archivo != null)
            {

                byte[] bytesArchivo;
                byte[] aEscribir;
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:59679");
                    var postJob = client.PostAsJsonAsync<string>("api/Chat/GetFile", Archivo);
                    postJob.Wait();
                    var postResult = postJob.Result;
                    if (postResult.IsSuccessStatusCode)
                    {
                        var readJob = postResult.Content.ReadAsAsync<byte[]>();
                        readJob.Wait();
                        bytesArchivo = readJob.Result;
                        aEscribir = Libreria.Metodos.LZWDecompress(bytesArchivo);
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
                var postJob = client.PostAsJsonAsync<string[]>("api/Chat/GetMsgs", usuariosActivos);
                postJob.Wait();
                var postResult = postJob.Result;
                if (postResult.IsSuccessStatusCode)
                {
                    var readJob = postResult.Content.ReadAsAsync<List<MessagesModel>>();
                    readJob.Wait();
                    mensajs = readJob.Result;
                }
            }
            ViewBag.Matriz = mensajs.ToArray();
            return View();
        }
        public ActionResult Chats()
        {
            usuarioReceptor = obtainCookieUsuarios("UsuarioReceptor");
            usuarioEnControl = obtainCookieUsuarios("UsuarioControl");
            var mensajs = new List<MessagesModel>();
            using (var client = new HttpClient())
            {
                string[] usuariosActivos = { usuarioEnControl, usuarioReceptor };
                client.BaseAddress = new Uri("http://localhost:59679");
                var postJob = client.PostAsJsonAsync<string[]>("api/Chat/GetMsgs", usuariosActivos);
                postJob.Wait();
                var postResult = postJob.Result;
                if (postResult.IsSuccessStatusCode)
                {
                    var readJob = postResult.Content.ReadAsAsync<List<MessagesModel>>();
                    readJob.Wait();
                    mensajs = readJob.Result;
                }
            }
            ViewBag.Matriz = mensajs.ToArray();

            return View();
        }

        public ActionResult EnvioMensajes(IFormFile ArchivoImportado, string Mensaje)
        {
            usuarioReceptor = obtainCookieUsuarios("UsuarioReceptor");
            usuarioEnControl = obtainCookieUsuarios("UsuarioControl");
            var ObjetoMensaje = new MessagesModel();
            if (Mensaje != null && ArchivoImportado != null)
            {
                ObjetoMensaje.Mensaje = Mensaje;
                ObjetoMensaje.FechaEnvio = DateTime.Now;
                ObjetoMensaje.EmisorMsg = usuarioEnControl;
                ObjetoMensaje.ReceptorMsg = usuarioReceptor;
                var bytesArchivo = new List<byte>();
                using (var Lectura = new BinaryReader(ArchivoImportado.OpenReadStream()))
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
                    var postJob = client.PostAsJsonAsync<MessagesModel>("api/Chat/SendMessage", ObjetoMensaje);
                    postJob.Wait();
                    var postResult = postJob.Result;
                    if (postResult.IsSuccessStatusCode)
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
                using (var Lectura = new BinaryReader(ArchivoImportado.OpenReadStream()))
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
                    var postJob = client.PostAsJsonAsync<MessagesModel>("api/Chat/SendMessage", ObjetoMensaje);
                    postJob.Wait();
                    var postResult = postJob.Result;
                    if (postResult.IsSuccessStatusCode)
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
                    var postJob = client.PostAsJsonAsync<MessagesModel>("api/Chat/SendMessage", ObjetoMensaje);
                    postJob.Wait();
                    var postResult = postJob.Result;
                    if (postResult.IsSuccessStatusCode)
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
            usuarioReceptor = obtainCookieUsuarios("UsuarioReceptor");
            usuarioEnControl = obtainCookieUsuarios("UsuarioControl");
            string[] parameters = { usuarioEnControl, usuarioReceptor, Mensaje };

            var mensajs = new List<MessagesModel>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:59679");
                var postJob = client.PostAsJsonAsync<string[]>("api/Chat/GetMsgsParam", parameters);
                postJob.Wait();
                var postResult = postJob.Result;
                if (postResult.IsSuccessStatusCode)
                {
                    var readJob = postResult.Content.ReadAsAsync<List<MessagesModel>>();
                    readJob.Wait();
                    mensajs = readJob.Result;
                }
            }
            ViewBag.Matriz = mensajs.ToArray();

            return View();
        }
        public ActionResult BusquedaM()
        {
            usuarioReceptor = obtainCookieUsuarios("UsuarioReceptor");
            usuarioEnControl = obtainCookieUsuarios("UsuarioControl");
            var mensajs = new List<MessagesModel>();

            using (var client = new HttpClient())
            {
                string[] usuariosActivos = { usuarioEnControl, usuarioReceptor };
                client.BaseAddress = new Uri("http://localhost:59679");
                var postJob = client.PostAsJsonAsync<string[]>("api/Chat/GetMsgs", usuariosActivos);
                postJob.Wait();
                var postResult = postJob.Result;
                if (postResult.IsSuccessStatusCode)
                {
                    var readJob = postResult.Content.ReadAsAsync<List<MessagesModel>>();
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
            //Borrar cookies
            return RedirectToAction("Inicio");
        }

        public ActionResult DescargarArchivos(string nombre)
        {
            usuarioReceptor = obtainCookieUsuarios("UsuarioReceptor");
            usuarioEnControl = obtainCookieUsuarios("UsuarioControl");
            var nombreArchivo = nombre;
            //enivarlo

            return View();
        }

        public void cookieUsuarios(string marca, string contenido)
        {
            HttpContext.Response.Cookies.Append(marca, contenido, new CookieOptions()
            {
                Expires = DateTime.Now.AddDays(5)
            });
        }

        public string obtainCookieUsuarios(string marca)
        {
            return HttpContext.Request.Cookies[marca];
        }

        public void tokenCookie(string value, [FromBody]Dictionary<string, string> objeto)
        {
            var buffer = value.PadRight(64, ' ')
               .ToCharArray()
               .Select(x => Convert.ToByte(x))
               .ToArray();
            var hander = new JwtSecurityTokenHandler();
            var claims = objeto.Select(x => new Claim(x.Key, x.Value.ToString()));
            var description = new SecurityTokenDescriptor
            {
                Issuer = "todos",
                Audience = "audiencia",
                Expires = DateTime.UtcNow.AddSeconds(10),
                Subject = new ClaimsIdentity(claims),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(buffer), SecurityAlgorithms.HmacSha256)
            };
            var token = hander.CreateToken(description);
            var tokenstring = hander.WriteToken(token);

            HttpContext.Response.Cookies.Append("tokens", tokenstring, new CookieOptions()
            {
                Expires = DateTime.Now.AddDays(5)
            });
        }

        public bool validateCookie(string authToken, string usuario)
        {
            string token = HttpContext.Request.Cookies["tokens"];
            return ValidateToken(token, usuario);
        }

        public bool ValidateToken(string authToken, string usuario)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = GetValidationParameters(usuario);
            SecurityToken validatedToken;
            IPrincipal principal = tokenHandler.ValidateToken(authToken, validationParameters, out validatedToken);
            return true;
        }

        public TokenValidationParameters GetValidationParameters(string texto)
        {
            return new TokenValidationParameters()
            {
                ValidateLifetime = false, // Because there is no expiration in the generated token
                ValidateAudience = false, // Because there is no audiance in the generated token
                ValidateIssuer = false,   // Because there is no issuer in the generated token
                ValidIssuer = "Usuario",
                ValidAudience = texto,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)) // The same key as the one that generate the token
            };
        }



    }

}