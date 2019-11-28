using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;


namespace vistas.Controllers
{
    public class UsuariosController : Controller
    {
        // GET: Usuarios
        public ActionResult Index()
        {
            return View();
        }

        // GET: Usuarios/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Usuarios/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Usuarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Usuarios/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Usuarios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Usuarios/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Usuarios/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public ActionResult IngresoU()
        {
            return View();
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult IngresoU (IFormCollection collection, string Usuario, string Nombre, string Password)
        {
            var nombreUsuario = Nombre;
            var usuario = Usuario;
            var contrasenia = Password;
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
            //CIFRAR CONTRASENIA CON aNumber
            //ENVIAR DATOS A MAURICIO
            //ENVIAR AL USUARIO A LA PAGINA DE INICIO
           
            return View();
            
        }
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Inicio(IFormCollection collection, string Usuario,  string Password)
        {            
            var usuario = Usuario;
            var contrasenia = Password;
            
            //if RECIBO EL VISTO BUENO{
            //GENERAR EL TOKEN
            //DAR EL TOKEN AL USUARIO en forma de archivo y hacerlo descargable TokenUsuario.token. poner un popup que diga:
            //"el archivo que recibio es su llave. no lo modifique. uselo cuando la app se lo solicite"}
            //else {Enviar un popup que diga sorry usuario/password incorrecto}
            
             return View();
        }
        public ActionResult Inicio()
        {
            return View();
        }
        public ActionResult SalaDeChat()
        {
            //Recibir la lista de usuarios y generar una ista que uestre todos los usuarios. 
            //Poner un buscador para buscar el nombre del usuario con el que se desea tener una conversacion y un boton que envie esa info a la dattabase
            //mandarlo a la vista TOKEN
            
            return View();
        }
        public ActionResult Chats()
        {
            //envia el nombre del usuario con el que desea hablar y recibe Mauricio los mensajes de los usuarios
            //con esta info recibida mostrar los mensajes de los usarios coronologicamente, el nombre del uuario con el que se está charlando 
            //colocar el buscado de mensajes y buscador de archivos para descargar 
            //para los archivos: si existe solo hacer que se descargue. si no está popup "no existe"
            //para la busqueda intentar con una partial view sin salirse del chat y mostrar la lista de mensajes con el emisor y la hora. en ultima instancia será una vista normal que regrese a la sala de chat
            //Para el envio de mensajes y archivos enviar el mensaje y el boton enviar ue haga refresh a la paggina. igual poner un boton abajo para el refresh
            //para el envio de archivos colocar un boton "seleccionar archivo" para que reciba el archivo y un boton enviar "enviar archivo"
            //validar que si se da "enviar" y la caja de teto está vacia no enviar nada a Mauricio y solo refrescar la pagina
            //poner boton para regresar a la sala de chat 

            return View();
        }
        public ActionResult Tokens()
        {
            //pide al usuario que envie un archivo de tipo .token y validarlo con el usuario
            //si es valido enviarlo a la vista Chats con la info del usuario con el que desea hablar
            //si no popup"esto es invalido" y regresarlo a SalaDeChat
            return View();
        }
        }
}