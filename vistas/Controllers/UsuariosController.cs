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

namespace vistas.Controllers
{
    public class UsuariosController : Controller
    {
        static string usuarioEnControl;
        static string usuarioReceptor;
        static string usuarioTry;
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
            byte[] contrasenia1 = Libreria.Metodos.EncryptionZigZag(byteContrasenia.ToArray(), aNumber);

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
            //if RECIBO EL VISTO BUENO{
            //GENERAR EL TOKEN
            //DAR EL TOKEN AL USUARIO en forma de archivo y hacerlo descargable TokenUsuario.token. poner un popup que diga:
            //"el archivo que recibio es su llave. no lo modifique. uselo cuando la app se lo solicite"}
            //else {Enviar un popup que diga sorry usuario/password incorrecto}
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
                        usuarioEnControl = Usuario;
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
            Directory.CreateDirectory(@"/App_Data/ArchivosDescargas/");
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
                        using (var writestream = new FileStream(@"/App_Data/ArchivosDescargas/" + Archivo,FileMode.OpenOrCreate))
                        {
                            using (var writer = new BinaryWriter(writestream))
                            {
                                foreach (var item in aEscribir)
                                {
                                    writer.Write(item);
                                }
                            }
                        }
                        var FileVirtualPath = @"/App_Data/ArchivosDescargas/" + Archivo;
                        return File(FileVirtualPath, "application/force-download", Path.GetFileName(FileVirtualPath));
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
        public ActionResult Tokens(string usuario, IFormFile ArchivoToken)
        {
            if (ArchivoToken == null && usuario == null)
            {
                //error
            }
            else if (ArchivoToken == null && usuario != null)
            {
                usuarioTry = usuario;
            }
            else if (ArchivoToken != null && usuario == null)
            {
                if (true/*if existe el usuario*/)
                {
                    usuarioReceptor = usuarioTry;
                }
                else
                {
                    //error

                }
            }
            //pide al usuario que envie un archivo de tipo .token y validarlo con el usuario
            //si es valido enviarlo a la vista Chats con la info del usuario con el que desea hablar
            //si no popup"esto es invalido" y regresarlo a SalaDeChat
            return View();
        }
        [HttpPost]
        public ActionResult BusquedaM(string Mensaje)
        {
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
        public ActionResult DescargarArchivos(string nombre)
        {
            var nombreArchivo = nombre;
            //enivarlo

            return View();
        }

    }

}