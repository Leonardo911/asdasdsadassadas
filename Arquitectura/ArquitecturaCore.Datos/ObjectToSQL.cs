using System;
using System.Collections.Generic;
using System.Text;

namespace ArquitecturaCore.Datos
{
    /// <summary>
    /// Mapea objetos tipo businessObject en datos de SQL
    /// </summary>
    public class ObjectToSQL
    {
        #region metodos
        /// <summary>
        /// Regresa un arreglo de parametros de sql a partir de un objeto tipo business object.
        /// </summary>
        /// <param name="obj">objeto del que se obtenera los arreglos</param>
        /// <param name="tipoMapper"> tipo de sp para el cual se obtendran los parametros
        /// 0. Insert
        /// 1. Update
        /// 2. Delete
        /// 3. Fetch
        /// </param>
        /// <returns>Arreglo de parametros de sql</returns>
        public static void MapperAttributes(System.Data.SqlClient.SqlCommand cmd, object obj, byte tipoMapper)
        {
            try
            {
                if (obj != null)
                {
                    System.ComponentModel.PropertyDescriptorCollection propiedades = System.ComponentModel.TypeDescriptor.GetProperties(obj);
                    if (tipoMapper <= 1)
                        MapperForInsertAndUpdate(cmd, obj, propiedades, tipoMapper == 0);
                    else MapperForFetchAndDelete(cmd, obj, propiedades);


                    //ArrayList parametrosArray = (tipoMapper <= 1)
                    //    ? MapperForInsertAndUpdate(obj, propiedades, tipoMapper == 0)
                    //    : MapperForFetchAndDelete(obj, propiedades);
                    //SqlParameter[] pars = new SqlParameter[parametrosArray.Count];
                    //for (int i = 0; i < parametrosArray.Count; i++)
                    //    pars[i] = (SqlParameter)parametrosArray[i];
                    //return pars;
                    //c4fac8fb3c 
                }
            }
            catch (Exception ee)
            {
                throw new Exception(ee.Message);
            }
        }

        private static bool EsBusinessObject(System.ComponentModel.PropertyDescriptor propiedad)
        {
            Type tipo = propiedad.PropertyType;
            while (tipo.BaseType != null)
            {
                tipo = tipo.BaseType;
                if (tipo.Name == "BusinessObject") return true;
            }
            return false;
        }

        /// <summary>
        /// Regresa un array de parametros sql para procedimientos almacenados de insert o update
        /// </summary>
        /// <param name="obj">objeto del que se mapean sus propiedades</param>
        /// <param name="propiedades">propiedades del objeto</param>
        /// <param name="isInsert">indica si es para un sp de insertar</param>
        /// <returns>arreglo de parametros</returns>
        private static void MapperForInsertAndUpdate(System.Data.SqlClient.SqlCommand cmd, object obj, System.ComponentModel.PropertyDescriptorCollection propiedades, bool isInsert)
        {
            // mapea cada una de las propiedades del objeto
            foreach (System.ComponentModel.PropertyDescriptor propiedad in propiedades)
            {
                // verifica si el usuario valido la propiedad para agregarse al arreglo.
                if (propiedad.IsBrowsable && !propiedad.IsLocalizable
                    && propiedad.PropertyType.Name != "BusinessObjectCollection"
                    && !EsBusinessObject(propiedad))
                {
                    System.Data.SqlDbType sqlTipo = ObtenerTipoSQL(propiedad.PropertyType);
                    object valor = propiedad.GetValue(obj);
                    System.Data.SqlClient.SqlParameter par = new System.Data.SqlClient.SqlParameter("@" + propiedad.Name, sqlTipo);
                    par.Value = valor;
                    // si es para insert y es identidad marca el parametro como salida y entrada de datos
                    // para que regrese el valor del identity
                    if (isInsert)
                    {
                        // obtiene los atributos marcados por el usuario de la propiedad del objeto
                        System.ComponentModel.DataObjectFieldAttribute atributos = (System.ComponentModel.DataObjectFieldAttribute)propiedad.Attributes[typeof(System.ComponentModel.DataObjectFieldAttribute)];
                        if (atributos != null && atributos.IsIdentity)
                        {
                            par.Direction = System.Data.ParameterDirection.Output;
                            if (par.DbType == System.Data.DbType.String)
                            {
                                par.Size = 100;
                            }

                        }
                    }
                    cmd.Parameters.Add(par);
                }
            }
        }

        /// <summary>
        /// Regresa un array de parametros sql para procedimientos almacenados de fetch o delete
        /// </summary>
        /// <param name="obj">objeto del que se mapean sus propiedades</param>
        /// <param name="propiedades">propiedades del objeto</param>
        /// <returns>arreglo de parametros</returns>
        private static void MapperForFetchAndDelete(System.Data.SqlClient.SqlCommand cmd, object obj, System.ComponentModel.PropertyDescriptorCollection propiedades)
        {
            // mapea cada una de las propiedades del objeto
            foreach (System.ComponentModel.PropertyDescriptor propiedad in propiedades)
            {
                // verifica si el usuario valido la propiedad para agregarse al arreglo
                if (propiedad.IsBrowsable && !propiedad.IsLocalizable && propiedad.PropertyType.Name != "BaseColleccion")
                {
                    // obtiene los atributos marcados por el usuario de la propiedad del objeto
                    System.ComponentModel.DataObjectFieldAttribute atributos = (System.ComponentModel.DataObjectFieldAttribute)propiedad.Attributes[typeof(System.ComponentModel.DataObjectFieldAttribute)];
                    // solo agrega los atributos que son primary keys en la tabla marcados por el usuario
                    // estos son marcados por el usuario
                    if (atributos != null && atributos.PrimaryKey)
                    {
                        System.Data.SqlDbType sqlTipo = ObtenerTipoSQL(propiedad.PropertyType);
                        object valor = propiedad.GetValue(obj);
                        System.Data.SqlClient.SqlParameter par = new System.Data.SqlClient.SqlParameter(propiedad.Name, sqlTipo);
                        par.Value = valor;
                        cmd.Parameters.Add(par);
                    }
                }
            }
        }
        /// <summary>
        /// Si el valor de la DB viene nulo, aqui agarra un valor por default
        /// </summary>
        /// <param name="tipo"></param>
        /// <returns></returns>
        private static object AsignarValorNulo(Type tipo)
        {
            if (tipo != null)
            {
                switch (tipo.ToString())
                {

                    case "Byte[]": return new Byte[] { };
                    case "Boolean": return false;
                    case "Byte": return Byte.MinValue;
                    case "Int16": return short.MinValue;
                    case "Int32": return int.MinValue; ;
                    case "Int64": return long.MinValue;
                    case "Decimal": return decimal.MinValue;
                    case "Single": return Single.MinValue;
                    case "Double": return double.MinValue;
                    case "DateTime": return DateTime.MinValue;
                    case "TimeSpan": return TimeSpan.MinValue;
                    default: return string.Empty; ;
                }
            }
            return null;
        }
        /// <summary>
        /// obtiene el tipo de dato sql de un tipo de una propiedad de un objeto en c#
        /// </summary>
        /// <param name="tipo"></param>
        /// <returns></returns>
        private static System.Data.SqlDbType ObtenerTipoSQL(Type tipo)
        {
            if (tipo != null)
            {
                System.Data.SqlDbType sqlType;
                if (tipo.IsEnum) return System.Data.SqlDbType.TinyInt;
                else
                {
                    switch (tipo.Name)
                    {
                        case "Byte[]": sqlType = System.Data.SqlDbType.Image; break;
                        case "Boolean": sqlType = System.Data.SqlDbType.Bit; break;
                        case "Byte": sqlType = System.Data.SqlDbType.TinyInt; break;
                        case "Int16": sqlType = System.Data.SqlDbType.SmallInt; break;
                        case "Int32": sqlType = System.Data.SqlDbType.Int; break;
                        case "Int64": sqlType = System.Data.SqlDbType.BigInt; break;
                        case "Decimal": sqlType = System.Data.SqlDbType.Decimal; break;
                        case "Single": sqlType = System.Data.SqlDbType.Real; break;
                        case "Double": sqlType = System.Data.SqlDbType.Float; break;
                        case "DateTime": sqlType = System.Data.SqlDbType.SmallDateTime; break;
                        case "String": sqlType = System.Data.SqlDbType.NVarChar; break;
                        case "TimeSpan": sqlType = System.Data.SqlDbType.Time; break;
                        default: sqlType = System.Data.SqlDbType.Variant; break;
                    }
                    return sqlType;
                }
            }
            else return System.Data.SqlDbType.Variant;
        }
        #endregion
    }
}
