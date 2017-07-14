using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CuyStore.Models;
using System.IO;

namespace CuyStore.Areas.admin.Controllers
{
    public class ProductoController : Controller
    {
        private StoreEntities db = new StoreEntities();

        // GET: admin/Producto
        public ActionResult Index()
        {
            var producto = db.Producto.Include(p => p.Categoria).Include(p => p.Proveedor);
            return View(producto.ToList());
        }

        // GET: admin/Producto/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Producto producto = db.Producto.Find(id);
            if (producto == null)
            {
                return HttpNotFound();
            }
            return View(producto);
        }

        // GET: admin/Producto/Create
        public ActionResult Create()
        {
            ViewBag.CategoriaId = new SelectList(db.Categoria, "CategoriaId", "Nombre");
            ViewBag.ProveedorId = new SelectList(db.Proveedor, "ProveedorId", "Nombre");
            return View();
        }

        // POST: admin/Producto/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductoId,CategoriaId,ProveedorId,Denominacion,Descripcion,PrecioUnitario,Existencias,Estado")] Producto producto)
        {
            if (ModelState.IsValid)
            {
                db.Producto.Add(producto);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategoriaId = new SelectList(db.Categoria, "CategoriaId", "Nombre", producto.CategoriaId);
            ViewBag.ProveedorId = new SelectList(db.Proveedor, "ProveedorId", "Nombre", producto.ProveedorId);
            return View(producto);
        }

        // GET: admin/Producto/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Producto producto = db.Producto.Find(id);
            if (producto == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoriaId = new SelectList(db.Categoria, "CategoriaId", "Nombre", producto.CategoriaId);
            ViewBag.ProveedorId = new SelectList(db.Proveedor, "ProveedorId", "Nombre", producto.ProveedorId);
            return View(producto);
        }

        // POST: admin/Producto/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductoId,CategoriaId,ProveedorId,Denominacion,Descripcion,PrecioUnitario,Existencias,Estado")] Producto producto)
        {
            if (ModelState.IsValid)
            {
                db.Entry(producto).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoriaId = new SelectList(db.Categoria, "CategoriaId", "Nombre", producto.CategoriaId);
            ViewBag.ProveedorId = new SelectList(db.Proveedor, "ProveedorId", "Nombre", producto.ProveedorId);
            return View(producto);
        }

        // GET: admin/Producto/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Producto producto = db.Producto.Find(id);
            if (producto == null)
            {
                return HttpNotFound();
            }
            return View(producto);
        }

        // POST: admin/Producto/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Producto producto = db.Producto.Find(id);
            db.Producto.Remove(producto);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public JsonResult Adjuntar(int ProductoId, HttpPostedFileBase documento)
        {
            var respuesta = new Models.ResponseModel
            {
                respuesta = true,
                error = ""
            };

            if (documento != null)
            {
                string adjunto = DateTime.Now.ToString("yyyyMMddHHmmss") + Path.GetExtension(documento.FileName);
                documento.SaveAs(Server.MapPath("~/ImgProductos/" + adjunto));

                db.ProductoImagen.Add(new ProductoImagen { ProductoId = ProductoId, Imagen = adjunto, Titulo = "Ejemplo", Descripcion = "Ejemplo" });
                db.SaveChanges();

            }
            else
            {
                respuesta.respuesta = false;
                respuesta.error = "Debe adjuntar un documento";
            }

            return Json(respuesta);
        }

        public PartialViewResult Adjuntos(int ProductoId)
        {
            return PartialView(db.ProductoImagen.Where(x => x.ProductoId == ProductoId).ToList());
        }

        public JsonResult EliminarImagen(int ProductoImagenId)
        {
            var rpt = new Models.ResponseModel()
            {
                respuesta = true,
                error = ""
            };
            var img = db.ProductoImagen.Find(ProductoImagenId);

            if (System.IO.File.Exists(Server.MapPath("~/ImgProductos/" + img.Imagen)))
                System.IO.File.Delete(Server.MapPath("~/ImgProductos/" + img.Imagen));

            db.ProductoImagen.Remove(img);
            db.SaveChanges();

            return Json(rpt);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
