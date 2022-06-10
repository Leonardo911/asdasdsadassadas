using System;
using System.Collections.Generic;
using System.Text;

namespace ArquitecturaCore.Negocio
{
    [Serializable]
    public class BusinessRuleCollection : System.Collections.DictionaryBase
    {
        #region Le indica la llave del diccionario
        public BusinessRule this[string reglaNombre]
        {
            get { return (BusinessRule)Dictionary[reglaNombre]; }
            set { Dictionary[reglaNombre] = value; }
        }
        #endregion

        #region Metodos
        /// <summary>
        /// Si no la contiene, agrega una regla a la coleccion de reglas.
        /// </summary>
        /// <param name="reglaNombre">Nombre de la regla, que funcionara como llave.</param>
        /// <param name="mensaje">Mensaje para el usuario cuando se rompa la regla.</param>
        /// <param name="reglaRota">Cuando es verdadera se agtrega a la coleccion, si es falsa se quita de la coleccion.</param>
        public void Add(string reglaNombre, string mensaje, bool reglaRota)
        {
            if (!reglaRota)
                Dictionary.Remove(reglaNombre);
            else if (!Contains(reglaNombre))
                Dictionary.Add(reglaNombre, new BusinessRule(reglaNombre, mensaje, reglaRota));
        }
        /// <summary>
        /// Indica si en la coleccion de reglas existe una regla con la llave especificada
        /// </summary>
        /// <param name="reglaNombre">nombre que se le asigno a la regla que funciona como llave.</param>
        /// <returns>verdadero si la contiene, falso si no la contiene.</returns>
        public bool Contains(string reglaNombre)
        {
            return Dictionary.Contains(reglaNombre);
        }
        #endregion

        #region Propiedades
        /// <summary>
        /// Acceso a las llaves de la coleccion.
        /// </summary>
        public System.Collections.ICollection Keys
        {
            get { return Dictionary.Keys; }
        }
        #endregion
    }
}
