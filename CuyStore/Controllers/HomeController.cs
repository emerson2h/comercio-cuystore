using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CuyStore.Controllers
{
        public class HomeController : Controller
    {
        private Models.StoreEntities bd = new Models.StoreEntities();
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Buscar(string id="")
        {
            // logica de acceso a base de datos
            var productos = bd.Producto.Where(x => x.Denominacion.Contains(id)).Take(20).ToList();
            // lista de productos
            ViewBag.clave = id;
            return View(productos);
        }

        public ActionResult Contacto()
        {
            return View();
        }

        public ActionResult Nosotros()
        {
            return View();
        }
    }
}