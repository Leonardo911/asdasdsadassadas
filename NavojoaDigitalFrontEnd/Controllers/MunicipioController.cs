using Microsoft.AspNetCore.Mvc;
using NavojoaDigitalFrontEnd.Negocio.Municipio;

namespace NavojoaDigitalFrontEnd.Controllers
{
    public class MunicipioController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        #region Dependencias
        public IActionResult Dependencias()
        {
            return View();
        }

        public async Task<IActionResult> DependenciasGet()
        {
            try
            {
                var dependencias = await Task.Run(() => Dependencia.Select().Cast<Dependencia>());
                var model = (from idep in dependencias
                             select new
                             {
                                 Id = idep.Id,
                                 Clave = idep.Clave,
                                 Nombre = idep.Nombre,
                                 Activo = idep.Activo
                             }).ToList();

                return Json(new { success = true, model = model });
            }
            catch (Exception ee)
            {
                return Json(new { success = false, error = ee.Message });
            }
        }

        public IActionResult DependenciaSave(Dependencia item)
        {
            try
            {
                if(item.Id > 0)
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
                    Activo = item.Activo
                };

                return Json(new { success = true, model = model });
            }
            catch (Exception ee)
            {
                return Json(new { success = false, error = ee.Message });
            }
        }

        public IActionResult DependenciaDelete(int id)
        {
            try
            {
                if(id > 0)
                {
                    var dependencia = new Dependencia() { Id = id };
                    dependencia.MarkOld();
                    dependencia.DeleteObject();
                }

                return Json(new { success = true });
            }
            catch (Exception ee)
            {
                return Json(new { success = false, error = ee.Message });
            }
        }
        #endregion

        #region Puestos
        public IActionResult Puestos()
        {
            return View();
        }

        public async Task<IActionResult> PuestosGet()
        {
            try
            {
                var puestos = await Task.Run(() => Puesto.Select().Cast<Puesto>());
                var model = (from ip in puestos
                             select new
                             {
                                 Id = ip.Id,
                                 Nombre = ip.Nombre,
                                 DependenciaId = ip.DependenciaId,
                                 Activo = ip.Activo,
                                 DependenciaNombre = ip.DependenciaNombre
                             }).OrderBy(ip => ip.Nombre).ToList();

                var dependencias = await Task.Run(() => Dependencia.Select().Cast<Dependencia>());
                var dependenciasModel = (from id in dependencias
                                         select new
                                         {
                                             value = id.Id,
                                             label = id.Nombre,
                                             selected = false
                                         }).OrderBy(id => id.value).ToList();

                return Json(new { success = true, model = model, dependencias = dependenciasModel });
            }
            catch (Exception ee)
            {
                return Json(new { success = false, error = ee.Message });
            }
        }

        public IActionResult PuestoSave(Puesto item)
        {
            try
            {
                if (item.Id > 0)
                {
                    item.MarkOld();
                    item.SetDirty(true);
                }

                if (item.DependenciaId <= 0)
                    throw new Exception("Debe indicar la dependencia del puesto");

                item.Save();

                var model = new
                {
                    Id = item.Id,
                    Nombre = item.Nombre,
                    DependenciaId = item.DependenciaId,
                    Activo = item.Activo,
                    DependenciaNombre = item.DependenciaNombre
                };

                return Json(new { success = true, model = model });
            }
            catch (Exception ee)
            {
                return Json(new { success = false, error = ee.Message });
            }
        }

        public IActionResult PuestoDelete(int id)
        {
            try
            {
                if (id > 0)
                {
                    var puesto = new Puesto() { Id = id };
                    puesto.MarkOld();
                    puesto.DeleteObject();
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
