using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Reflection;
using ArquitecturaCore.Datos;

namespace ArquitecturaCore.Negocio
{
    public class XMLMapper
    {
        #region Metodos
        /// <summary>
        /// Asigna un valor a una propiedad
        /// </summary>
        /// <param name="businessObject">businessObject al cual se le va a asignar la propiedad</param>
        /// <param name="valueStr">valor a asignar</param>
        /// <param name="field">propiedad a la cual se le va a asignar el valor</param>
        private static void AsignarValorAPropiedad(BusinessObject businessObject, string valueStr, PropertyInfo field)
        {
            if (field != null && field.PropertyType != null)
            {
                // si la propiedad es una enumeracion toma este camino especial
                if (field.PropertyType.IsEnum)
                {
                    object en = Enum.Parse(field.PropertyType, valueStr);
                    field.SetValue(businessObject, en, null);
                }
                else
                {
                    // asigna el valor haciendo parses segun el tipo de propiedad que es
                    switch (field.PropertyType.Name)
                    {
                        //case "Byte[]":
                        case "Boolean": field.SetValue(businessObject, (valueStr == "1") ? true : false, null); break;
                        case "Byte": field.SetValue(businessObject, byte.Parse(valueStr), null); break;
                        case "Byte[]":
                            byte[] arreglo = Convert.FromBase64String(valueStr);
                            field.SetValue(businessObject, arreglo, null); break;
                        case "Int16": field.SetValue(businessObject, short.Parse(valueStr), null); break;
                        case "Int32": field.SetValue(businessObject, int.Parse(valueStr), null); break;
                        case "Int64": field.SetValue(businessObject, long.Parse(valueStr), null); break;
                        case "Decimal": field.SetValue(businessObject, decimal.Parse(valueStr), null); break;
                        case "Single": field.SetValue(businessObject, float.Parse(valueStr), null); break;
                        case "Double": field.SetValue(businessObject, double.Parse(valueStr), null); break;
                        case "DateTime": field.SetValue(businessObject, DateTime.Parse(valueStr), null); break;
                        case "String": field.SetValue(businessObject, valueStr, null); break;
                        case "TimeSpan": TimeSpan ts = TimeSpan.Parse(valueStr); field.SetValue(businessObject, ts, null); break;
                    }
                }
            }
        }
        /// <summary>
        /// Mapea un solo businessObject con los valores del sp a n niveles de asignacion
        /// </summary>
        /// <param name="node">nodo con los valores</param>
        /// <param name="businessObject">businessObject al que se van a asignar los valores</param>
        public static void XMLToBusinessObject(XmlNode node, BusinessObject businessObject)
        {
            // obtiene el tipo del businessIbject
            Type businessObjectType = businessObject.GetType();
            businessObject.SetSeEstaCargando();
            // asigna el valor del xml a los atributos del businessObject
            foreach (XmlAttribute attr in node.Attributes)
            {
                // obtiene la propiedad a asignar el valor y se lo asigna a la propiedad del objeto
                PropertyInfo prop = businessObjectType.GetProperty(attr.Name);
                if (prop != null)
                    AsignarValorAPropiedad(businessObject, attr.Value, prop);
            }
            if (node.HasChildNodes) XMLToBusinessObjectChild(node, businessObject);
            businessObject.MarkOld();
        }
        /// <summary>
        /// Mapea un objeto con todas sus propiedaes a n niveles del objeto
        /// a partir de una tabla XML
        /// </summary>
        /// <param name="reader">reader devuelto por el sp</param>
        /// <param name="objeto">objeto ya instanciado con el que se va a mapear</param>
        public static void XMLToBusinessObject(XmlDocument doc, object objeto)
        {
            // se hace el cast del businessobject
            BusinessObject businessObject = (BusinessObject)objeto;
            businessObject.SetSeEstaCargando();
            // el reader se pasa a xmldocument
            // se mapea cada uno de los hijos del documento con las propiedades
            if (doc.HasChildNodes)
                foreach (XmlNode node in doc.ChildNodes)
                    XMLToBusinessObject(node, businessObject);
            businessObject.MarkOld();
        }
        /// <summary>
        /// Mapea los valores del sp con los atributos de los hijos del businessObject
        /// </summary>
        /// <param name="node">nodo a partir del que se van a obtener los valores</param>
        /// <param name="businessObject">business object al que se van a asignar los hijos</param>
        public static void XMLToBusinessObjectChild(XmlNode node, BusinessObject businessObject)
        {
            // se obtiene el tipo del businesObject
            Type businessObjectType = businessObject.GetType();
            // se recorren todos los hijos del nodo para asignarle sus propiedades
            foreach (XmlNode child in node.ChildNodes)
            {
                // se obtiene la propiedad hijo del objeto
                PropertyInfo prop = businessObjectType.GetProperty(child.Name);
                if (prop == null)
                {
                    Console.Write(child.Name);
                    continue;
                }
                // si es una coleccion se le asignan sus valores
                if (prop.PropertyType.Name == "BusinessObjectCollection")
                {
                    // se obtienen el atributo de coleccion que puso el programador en el objeto
                    object[] atributos = prop.GetCustomAttributes(typeof(MapperBusinessObjectCollectionAttribute), true);
                    if (atributos.Length > 0)
                    {
                        // se obtiene el tipo del hijo del valor del atributo y se instancia el objeto nuevo
                        Type childType = ((MapperBusinessObjectCollectionAttribute)atributos[0]).Tipo;
                        BusinessObject bOchild = (BusinessObject)Activator.CreateInstance(childType);
                        // se le asignan los valores a las propiedades del objeto
                        XMLToBusinessObject(child, bOchild);
                        // se obtiene la coleccion de la propiedad del objeto y se agrega el hijo
                        BusinessObjectCollection col = (BusinessObjectCollection)prop.GetValue(businessObject, null);
                        col.Add(bOchild);
                        businessObject.AgregaEventosColOnLoad(col);
                    }
                }
                else
                {
                    // se obtienen los atributos de business object de la propiedad
                    object[] atributos = prop.GetCustomAttributes(typeof(MapperBusinessObjectAttribute), true);
                    // si es mayor de cero entonces es un businessObject
                    if (atributos.Length > 0)
                    {
                        // se instancia el businessObject
                        BusinessObject bOchild = (BusinessObject)Activator.CreateInstance(prop.PropertyType);
                        // se le asignan los valores de sus propiedades
                        XMLToBusinessObject(child, bOchild);
                        // se asigna el hijo al businessObject
                        prop.SetValue(businessObject, bOchild, null);
                    }
                }
            }
        }

        public static void XMLToBusinessObjectCollection(XmlDocument doc, BusinessObjectCollection col, Type tipo)
        {
            //while (reader.Read())
            //{
            //XmlDocument doc = new XmlDocument();
            //doc.Load(reader);
            if (doc.HasChildNodes)
                foreach (XmlNode node in doc.ChildNodes)
                    XMLToBusinessObjectCollection(node, col, tipo);
            //}
        }

        private static void XMLToBusinessObjectCollection(XmlNode node, BusinessObjectCollection col, Type tipo)
        {
            // se recorren todos los hijos del nodo para asignarle sus propiedades
            foreach (XmlNode child in node.ChildNodes)
            {
                // se instancia el businessObject
                BusinessObject bOchild = (BusinessObject)Activator.CreateInstance(tipo);
                // se le asignan los valores de sus propiedades
                XMLToBusinessObject(child, bOchild);
                col.Add(bOchild);
            }
        }
        #endregion
    }
}
