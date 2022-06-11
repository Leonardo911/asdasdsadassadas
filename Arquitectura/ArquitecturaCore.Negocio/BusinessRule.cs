using System;
using System.Collections.Generic;
using System.Text;

namespace ArquitecturaCore.Negocio
{
    public class BusinessRule
    {
        #region Constructor
        public BusinessRule(string nombre, string mensaje, bool reglaRota)
        {
            _Nombre = nombre;
            _Mensaje = mensaje;
            _ReglaRota = reglaRota;
        }
        #endregion

        #region Propiedades
        private string _Nombre = string.Empty;
        /// <summary>
        /// Nombre de la regla.
        /// </summary>
        public string Nombre
        {
            get { return _Nombre; }
            set { _Nombre = value; }
        }

        private string _Mensaje = string.Empty;
        /// <summary>
        /// Mensaje que le aparece al usuario cuando la regla se rompe.
        /// </summary>
        public string Mensaje
        {
            get { return _Mensaje; }
            set { _Mensaje = value; }
        }

        private bool _ReglaRota = false;
        /// <summary>
        /// Si es verdadero se agrega a la coleccion de reglas, si es falso se quita.
        /// </summary>
        public bool ReglaRota
        {
            get { return _ReglaRota; }
            set { _ReglaRota = value; }
        }
        #endregion
    }
}
