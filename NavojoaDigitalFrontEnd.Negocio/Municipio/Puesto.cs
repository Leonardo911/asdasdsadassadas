using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArquitecturaCore.Negocio;

namespace NavojoaDigitalFrontEnd.Negocio.Municipio
{
    public class Puesto : BusinessObject
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

        private string _Nombre = string.Empty;
        public string Nombre
        {
            get { return _Nombre; }
            set
            {
                if (AsignaPropiedadString(_Nombre, ref value))
                {
                    if (CheckRule("El campo no debe ser mayor de 200 caracteres", value.Trim().Length > 200))
                    {
                        _Nombre = value.Trim().ToUpper();
                        SetDirty(true);
                    }
                }
            }
        }

        private int _DependenciaId = -1;
        public int DependenciaId
        {
            get { return _DependenciaId; }
            set
            {
                if (_DependenciaId != value)
                {
                    _DependenciaId = value;
                    SetDirty(true);
                }
            }
        }

        private bool _Activo = false;
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

        #region localizables
        [System.ComponentModel.Localizable(true)]
        public string DependenciaNombre { get; set; } = string.Empty;
        #endregion

        #region metodos
        public static BusinessObjectCollection Select(int dependenciaId)
        {
            return BusinessObjectFacade.SelectObjects(typeof(Puesto), "municipio.PuestoSelect", dependenciaId);
        }

        public static BusinessObjectCollection Select()
        {
            return Select(-1);
        }
        #endregion

        #region reglas
        protected override void AgregaReglas()
        {
            Reglas.Add("NombreVacio", "Debe especificar el campo Nombre", _Nombre.Trim().Length == 0);
            Reglas.Add("DependenciaIdVacio", "Debe especificar el campo DependenciaId", _DependenciaId <= 0);
        }
        #endregion
    }
}
