using System;
using System.Collections.Generic;
using System.Text;

namespace ArquitecturaCore.Negocio
{
    /// <summary>
    /// Atributo para indicar que es coleccion a la hora de mapear con ablas xml.
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public sealed class MapperBusinessObjectCollectionAttribute : Attribute
    {
        #region propiedades
        private Type _Tipo;
        /// <summary>
        /// Tipo de businessObject con el que se va a llenar la coleccion o el objeto.
        /// </summary>
        public Type Tipo
        {
            get { return _Tipo; }
            set { _Tipo = value; }
        }
        #endregion

        #region constructor
        public MapperBusinessObjectCollectionAttribute(Type tipo)
        {
            _Tipo = tipo;
        }
        #endregion
    }

    [AttributeUsage(AttributeTargets.All)]
    public sealed class MapperBusinessObjectAttribute : Attribute
    {
        #region constructor
        public MapperBusinessObjectAttribute()
        {
        }
        #endregion
    }
}
