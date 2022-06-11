using Microsoft.AspNetCore.Mvc;
using NavojoaDigitalFrontEnd.Negocio.Compras;

namespace NavojoaDigitalFrontEnd.Controllers
{
    public class ComprasController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        #region productos
        public IActionResult Productos()
        {
            return View("Productos");
        }
        public async Task<IActionResult> ProductosGet()
        {
            var productos = await Task.Run(() => Producto.Select().Cast<Producto>());
            var model = (from iproducto in productos
                         select new
                         {
                             Id = iproducto.Id,
                             Clave = iproducto.Clave,
                             Nombre = iproducto.Nombre,
                             Descripcion = iproducto.Descripcion,
                             PartidaDetalleId = iproducto.PartidaDetalleId,
                             SKU = iproducto.SKU,
                             Unidad = iproducto.Unidad,
                             Marca = iproducto.Marca,
                             Modelo = iproducto.Modelo,
                             ProveedorIdUltimo = iproducto.ProveedorIdUltimo,
                             PrecioUltimo = iproducto.PrecioUltimo,
                             CategoriaId = iproducto.CategoriaId,
                             PartidaDetalleNombre = iproducto.PartidaDetalleNombre,
                             ProveedorNombreUltimo = iproducto.ProveedorNombreUltimo,
                             CategoriaNombre = iproducto.CategoriaNombre
                         }).ToList();

            var categorias = await Task.Run(() => Categoria.Select().Cast<Categoria>());
            var catsModel = (from icat in categorias
                             select new
                             {
                                 Id = icat.Id,
                                 Nombre = icat.Nombre
                             }).ToList();

            return Json(new { success = true, productos = model, categorias = catsModel });
        }
        public IActionResult ProductoSave(Producto item)
        {
            try
            {
                if (item.Id > 0)
                {
                    item.MarkOld();
                    item.SetDirty(true);
                }
                item.Save();
                var model = new
                {
                    Id = item.Id,
                    Clave = item.Clave,
                    Nombre = item.Nombre,
                    Descripcion = item.Descripcion,
                    PartidaDetalleId = item.PartidaDetalleId,
                    SKU = item.SKU,
                    Unidad = item.Unidad,
                    Marca = item.Marca,
                    Modelo = item.Modelo,
                    ProveedorIdUltimo = item.ProveedorIdUltimo,
                    PrecioUltimo = item.PrecioUltimo,
                    CategoriaId = item.CategoriaId,
                    PartidaDetalleNombre = item.PartidaDetalleNombre,
                    ProveedorNombreUltimo = item.ProveedorNombreUltimo,
                    CategoriaNombre = item.CategoriaNombre
                };

                return Json(new { success = true, model = model });
            }
            catch (Exception ee)
            {
                return Json(new { success = false, error = ee.Message });
            }
        }

        public IActionResult ProductoDelete(int id)
        {
            try
            {
                if(id > 0)
                {
                    var producto = new Producto() { Id = id };
                    producto.MarkOld();
                    producto.DeleteObject();
                }

                return Json(new { success = true });
            }
            catch (Exception ee)
            {
                return Json(new { success = false, error = ee.Message });
            }
        }
        #endregion

        #region servicios
        public IActionResult Servicios()
        {
            return View("Servicios");
        }
        public async Task<IActionResult> ServiciosGet()
        {
            try
            {
                var servicios = await Task.Run(() => Servicio.Select().Cast<Servicio>());
                var model = (from iservicio in servicios
                             select new
                             {
                                 Id = iservicio.Id,
                                 Clave = iservicio.Clave,
                                 Nombre = iservicio.Nombre,
                                 Descripcion = iservicio.Descripcion,
                                 PartidaDetalleId = iservicio.PartidaDetalleId,
                                 PartidaDetalleNombre = iservicio.PartidaDetalleNombre
                             }).ToList();

                return Json(new { success = true, servicios = model });
            }
            catch(Exception ee)
            {
                return Json(new { success = false, error = ee.Message });
            }
        }
        public IActionResult ServicioSave(Servicio item)
        {
            try
            {
                if (item.Id > 0)
                {
                    item.MarkOld();
                    item.SetDirty(true);
                }
                item.Save();
                var model = new
                {
                    Id = item.Id,
                    Clave = item.Clave,
                    Nombre = item.Nombre,
                    Descripcion = item.Descripcion,
                    PartidaDetalleId = item.PartidaDetalleId,
                    PartidaDetalleNombre = item.PartidaDetalleNombre
                };

                return Json(new { success = true, model = model });
            }
            catch (Exception ee)
            {
                return Json(new { success = false, error = ee.Message });
            }
        }
        public IActionResult ServicioDelete(int id)
        {
            try
            {
                if (id > 0)
                {
                    var servicio = new Servicio() { Id = id };
                    servicio.MarkOld();
                    servicio.DeleteObject();
                }

                return Json(new { success = true });
            }
            catch (Exception ee)
            {
                return Json(new { success = false, error = ee.Message });
            }
        }
        #endregion

        #region categorias
        public IActionResult Categorias()
        {
            return View("Categorias");
        }

        public async Task<IActionResult> CategoriasGet()
        {
            try
            {
                var categorias = await Task.Run(() => Categoria.Select().Cast<Categoria>());
                var model = (from icategoria in categorias
                             select new
                             {
                                 Id = icategoria.Id,
                                 Nombre = icategoria.Nombre,
                             }).ToList();

                return Json(new { success = true, model = model });
            }
            catch (Exception ee)
            {
                return Json(new { success = false, error = ee.Message });
            }
        }

        public IActionResult CategoriaSave(Categoria item)
        {
            try
            {
                if (item.Id > 0)
                {
                    item.MarkOld();
                    item.SetDirty(true);
                }
                item.Save();
                var model = new
                {
                    Id = item.Id,
                    Nombre = item.Nombre,
                };

                return Json(new { success = true, model = model });
            }
            catch (Exception ee)
            {
                return Json(new { success = false, error = ee.Message });
            }
        }

        public IActionResult CategoriaDelete(int id)
        {
            try
            {
                if (id > 0)
                {
                    var categoria = new Categoria() { Id = id };
                    categoria.MarkOld();
                    categoria.DeleteObject();
                }

                return Json(new { success = true });
            }
            catch (Exception ee)
            {
                return Json(new { success = false, error = ee.Message });
            }
        }
        #endregion
    }
}
