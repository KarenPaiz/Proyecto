using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

                return View();
            
        }
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Inicio(IFormCollection collection, string Usuario,  string Password)
        {
            
            var usuario = Usuario;
            var contrasenia = Password;
            Random rnd = new Random();
            int aNumber = rnd.Next(1, 100);
            return View();

        }
        public ActionResult Inicio()
        {

            return View();

        }
        public ActionResult SalaDeChat()
        {

            return View();

        }

    }
}