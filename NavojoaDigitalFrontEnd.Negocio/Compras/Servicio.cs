using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArquitecturaCore.Negocio;

namespace NavojoaDigitalFrontEnd.Negocio.Compras
{
    public class Servicio : BusinessObject
    {
        #region propiedades
        private int _Id = -1;
        [System.ComponentModel.DataObjectField(true, true)]
        public int Id
        {
            get { return _Id; }
            set
            {
                if (_Id != value)
                {
                    _Id = value;
                    SetDirty(true);
                }
            }
        }

        private string _Clave = string.Empty;
        public string Clave
        {
            get { return _Clave; }
            set
            {
                if (AsignaPropiedadString(_Clave, ref value))
                {
                    if (CheckRule("El campo no debe ser mayor de 50 caracteres", value.Trim().Length > 50))
                    {
                        _Clave = value.Trim();
                        SetDirty(true);
                    }
                }
            }
        }

        private string _Nombre = string.Empty;
        public string Nombre
        {
            get { return _Nombre; }
            set
            {
                if (AsignaPropiedadString(_Nombre, ref value))
                {
                    _Nombre = value.Trim();
                    SetDirty(true);
                }
            }
        }

        private string _Descripcion = string.Empty;
        public string Descripcion
        {
            get { return _Descripcion; }
            set
            {
                if (AsignaPropiedadString(_Descripcion, ref value))
                {
                    _Descripcion = value.Trim();
                    SetDirty(true);
                }
            }
        }

        private int _PartidaDetalleId = -1;
        public int PartidaDetalleId
        {
            get { return _PartidaDetalleId; }
            set
            {
                if (_PartidaDetalleId != value)
                {
                    _PartidaDetalleId = value;
                    SetDirty(true);
                }
            }
        }
        #endregion

        #region localizables
        [System.ComponentModel.Localizable(true)]
        public string PartidaDetalleNombre { get; set; } = string.Empty;
        #endregion

        #region metodos
        public static BusinessObjectCollection Select()
        {
            return BusinessObjectFacade.SelectObjects(typeof(Servicio), "compras.ServicioSelect");
        }
        #endregion

        #region reglas
        protected override void AgregaReglas()
        {
            Reglas.Add("ClaveVacio", "Debe especificar el campo Clave", _Clave.Trim().Length == 0);
            Reglas.Add("NombreVacio", "Debe especificar el campo Nombre", _Nombre.Trim().Length == 0);
        }
        #endregion
    }
}
