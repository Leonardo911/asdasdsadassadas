using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArquitecturaCore.Negocio;

namespace NavojoaDigitalFrontEnd.Negocio.Proveedores
{
    public class Proveedor : BusinessObject
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

        private string _NombreComercial = string.Empty;
        public string NombreComercial
        {
            get { return _NombreComercial; }
            set
            {
                if (AsignaPropiedadString(_NombreComercial, ref value))
                {
                    if (CheckRule("El campo no debe ser mayor de -1 caracteres", value.Trim().Length > -1))
                    {
                        _NombreComercial = value.Trim().ToUpper();
                        SetDirty(true);
                    }
                }
            }
        }

        private string _RazonSocial = string.Empty;
        public string RazonSocial
        {
            get { return _RazonSocial; }
            set
            {
                if (AsignaPropiedadString(_RazonSocial, ref value))
                {
                    if (CheckRule("El campo no debe ser mayor de -1 caracteres", value.Trim().Length > -1))
                    {
                        _RazonSocial = value.Trim().ToUpper();
                        SetDirty(true);
                    }
                }
            }
        }

        private string _RFC = string.Empty;
        public string RFC
        {
            get { return _RFC; }
            set
            {
                if (AsignaPropiedadString(_RFC, ref value))
                {
                    if (CheckRule("El campo no debe ser mayor de 50 caracteres", value.Trim().Length > 50))
                    {
                        _RFC = value.Trim().ToUpper();
                        SetDirty(true);
                    }
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

        private string _Direccion = string.Empty;
        public string Direccion
        {
            get { return _Direccion; }
            set
            {
                if (AsignaPropiedadString(_Direccion, ref value))
                {
                    if (CheckRule("El campo no debe ser mayor de -1 caracteres", value.Trim().Length > -1))
                    {
                        _Direccion = value.Trim().ToUpper();
                        SetDirty(true);
                    }
                }
            }
        }

        private string _CP = string.Empty;
        public string CP
        {
            get { return _CP; }
            set
            {
                if (AsignaPropiedadString(_CP, ref value))
                {
                    if (CheckRule("El campo no debe ser mayor de 5 caracteres", value.Trim().Length > 5))
                    {
                        _CP = value.Trim().ToUpper();
                        SetDirty(true);
                    }
                }
            }
        }

        private string _Telefono = string.Empty;
        public string Telefono
        {
            get { return _Telefono; }
            set
            {
                if (AsignaPropiedadString(_Telefono, ref value))
                {
                    if (CheckRule("El campo no debe ser mayor de 50 caracteres", value.Trim().Length > 50))
                    {
                        _Telefono = value.Trim().ToUpper();
                        SetDirty(true);
                    }
                }
            }
        }

        private string _Contacto = string.Empty;
        public string Contacto
        {
            get { return _Contacto; }
            set
            {
                if (AsignaPropiedadString(_Contacto, ref value))
                {
                    if (CheckRule("El campo no debe ser mayor de 400 caracteres", value.Trim().Length > 400))
                    {
                        _Contacto = value.Trim().ToUpper();
                        SetDirty(true);
                    }
                }
            }
        }

        private DateTime _FechaAlta = DateTime.Now;
        public DateTime FechaAlta
        {
            get { return _FechaAlta; }
            set
            {
                if (_FechaAlta != value)
                {
                    _FechaAlta = value;
                    SetDirty(true);
                }
            }
        }

        private string _DescripcionServicio = string.Empty;
        public string DescripcionServicio
        {
            get { return _DescripcionServicio; }
            set
            {
                if (AsignaPropiedadString(_DescripcionServicio, ref value))
                {
                    if (CheckRule("El campo no debe ser mayor de -1 caracteres", value.Trim().Length > -1))
                    {
                        _DescripcionServicio = value.Trim().ToUpper();
                        SetDirty(true);
                    }
                }
            }
        }

        private byte _Estatus = 0;
        public byte Estatus
        {
            get { return _Estatus; }
            set
            {
                if (_Estatus != value)
                {
                    _Estatus = value;
                    SetDirty(true);
                }
            }
        }

        private int _UsuarioIdAutorizo = -1;
        public int UsuarioIdAutorizo
        {
            get { return _UsuarioIdAutorizo; }
            set
            {
                if (_UsuarioIdAutorizo != value)
                {
                    _UsuarioIdAutorizo = value;
                    SetDirty(true);
                }
            }
        }

        #endregion

        #region reglas
        protected override void AgregaReglas()
        {
            
            Reglas.Add("NombreComercialVacio", "Debe especificar el campo NombreComercial", _NombreComercial.Trim().Length == 0);
            Reglas.Add("RazonSocialVacio", "Debe especificar el campo RazonSocial", _RazonSocial.Trim().Length == 0);
            Reglas.Add("RFCVacio", "Debe especificar el campo RFC", _RFC.Trim().Length == 0);
            Reglas.Add("ClaveVacio", "Debe especificar el campo Clave", _Clave.Trim().Length == 0);
            Reglas.Add("FechaAltaVacio", "Debe especificar el campo FechaAlta", _FechaAlta == DateTime.Now);
        }
        #endregion

    }
}
