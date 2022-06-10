using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArquitecturaCore.Negocio;

namespace NavojoaDigitalFrontEnd.Negocio.Compras
{
    public class Categoria : BusinessObject
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
                    if (CheckRule("El campo no debe ser mayor de 400 caracteres", value.Trim().Length > 400))
                    {
                        _Nombre = value.Trim();
                        SetDirty(true);
                    }
                }
            }
        }
        #endregion

        public static BusinessObjectCollection Select()
        {
            return BusinessObjectFacade.SelectObjects(typeof(Categoria), "compras.CategoriaSelect");
        }

        #region reglas
        protected override void AgregaReglas()
        {
            Reglas.Add("NombreVacio", "Debe especificar el nombre", _Nombre.Trim().Length == 0);
        }
        #endregion
    }
}
