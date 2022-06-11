using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArquitecturaCore.Negocio;

namespace NavojoaDigitalFrontEnd.Negocio.Municipio
{
    public class Dependencia : BusinessObject
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
                        _Clave = value.Trim().ToUpper();
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

        private bool _Activo = true;
        public bool Activo
        {
            get { return _Activo; }
            set
            {
                if (_Activo != value)
                {
                    _Activo = value;
                    SetDirty(true);
                }
            }
        }
        #endregion

        #region metodos
        public static BusinessObjectCollection Select()
        {
            return BusinessObjectFacade.SelectObjects(typeof(Dependencia), "municipio.DependenciaSelect");
        }
        #endregion

        #region reglas
        protected override void AgregaReglas()
        {
            Reglas.Add("ClaveVacio", "Debe especificar la clave", _Clave.Trim().Length == 0);
            Reglas.Add("NombreVacio", "Debe especificar el nombre", _Nombre.Trim().Length == 0);
        }
        #endregion
    }
}
