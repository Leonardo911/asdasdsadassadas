using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArquitecturaCore.Negocio;

namespace NavojoaDigitalFrontEnd.Negocio.Compras
{
    public class Producto : BusinessObject
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

        private string _SKU = string.Empty;
        public string SKU
        {
            get { return _SKU; }
            set
            {
                if (AsignaPropiedadString(_SKU, ref value))
                {
                    if (CheckRule("El campo no debe ser mayor de 200 caracteres", value.Trim().Length > 200))
                    {
                        _SKU = value.Trim();
                        SetDirty(true);
                    }
                }
            }
        }

        private string _Unidad = string.Empty;
        public string Unidad
        {
            get { return _Unidad; }
            set
            {
                if (AsignaPropiedadString(_Unidad, ref value))
                {
                    if (CheckRule("El campo no debe ser mayor de 200 caracteres", value.Trim().Length > 200))
                    {
                        _Unidad = value.Trim();
                        SetDirty(true);
                    }
                }
            }
        }

        private string _Marca = string.Empty;
        public string Marca
        {
            get { return _Marca; }
            set
            {
                if (AsignaPropiedadString(_Marca, ref value))
                {
                    if (CheckRule("El campo no debe ser mayor de 200 caracteres", value.Trim().Length > 200))
                    {
                        _Marca = value.Trim();
                        SetDirty(true);
                    }
                }
            }
        }

        private string _Modelo = string.Empty;
        public string Modelo
        {
            get { return _Modelo; }
            set
            {
                if (AsignaPropiedadString(_Modelo, ref value))
                {
                    if (CheckRule("El campo no debe ser mayor de 200 caracteres", value.Trim().Length > 200))
                    {
                        _Modelo = value.Trim();
                        SetDirty(true);
                    }
                }
            }
        }

        private int _ProveedorIdUltimo = -1;
        public int ProveedorIdUltimo
        {
            get { return _ProveedorIdUltimo; }
            set
            {
                if (_ProveedorIdUltimo != value)
                {
                    _ProveedorIdUltimo = value;
                    SetDirty(true);
                }
            }
        }

        private decimal _PrecioUltimo = 0;
        public decimal PrecioUltimo
        {
            get { return _PrecioUltimo; }
            set
            {
                if (_PrecioUltimo != value)
                {
                    _PrecioUltimo = value;
                    SetDirty(true);
                }
            }
        }

        private int _CategoriaId = -1;
        public int CategoriaId
        {
            get { return _CategoriaId; }
            set
            {
                if (_CategoriaId != value)
                {
                    _CategoriaId = value;
                    SetDirty(true);
                }
            }
        }
        #endregion

        #region localizables
        [System.ComponentModel.Localizable(true)]
        public string PartidaDetalleNombre { get; set; } = string.Empty;

        [System.ComponentModel.Localizable(true)]
        public string ProveedorNombreUltimo { get; set; } = string.Empty;

        [System.ComponentModel.Localizable(true)]
        public string CategoriaNombre { get; set; } = string.Empty;
        #endregion

        #region metodos
        public static BusinessObjectCollection Select()
        {
            return BusinessObjectFacade.SelectObjects(typeof(Producto), "compras.ProductoSelect");
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
