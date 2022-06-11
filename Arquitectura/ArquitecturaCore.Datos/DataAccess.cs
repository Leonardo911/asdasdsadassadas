using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Xml;
using System.Reflection;
using System.Data;

namespace ArquitecturaCore.Datos
{
    public class DataAccess
    {
        private static XmlDocument ExcecuteXmlReader(string spName, object[] values)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                try
                {
                    using (SqlConnection con = ObtenerCadena())
                    {
                        using (SqlCommand cmd = new SqlCommand(spName, con))
                        {
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                            con.Open();
                            if (values != null && values.Length > 0)
                            {
                                SqlCommandBuilder.DeriveParameters(cmd);
                                List<SqlParameter> parameters = new List<SqlParameter>();
                                foreach (SqlParameter parameter in cmd.Parameters)
                                    if (parameter.Direction == ParameterDirection.Input)
                                        parameters.Add(parameter);
                                for (int i = 0; i < values.Length; i++)
                                    parameters[i].Value = values[i];
                            }
                            doc.Load(cmd.ExecuteXmlReader());
                            con.Close();
                        }
                    }
                }
                catch (SqlException ee)
                {
                    throw new Exception(ee.Message);
                }
                return doc;
            }
            catch (Exception ee)
            {
                throw new Exception(ee.Message);
            }
        }

        private static void ExecuteNonQuery(System.Data.CommandType commandType, string query, object[] values)
        {
            try
            {
                try
                {
                    using (SqlConnection con = ObtenerCadena())
                    {
                        using (SqlCommand cmd = new SqlCommand(query, con))
                        {
                            // There're three command types: StoredProcedure, Text, TableDirect. The TableDirect   
                            // type is only for OLE DB.    
                            cmd.CommandType = commandType;
                            con.Open();

                            if (values != null && values.Length > 0)
                            {
                                SqlCommandBuilder.DeriveParameters(cmd);
                                List<SqlParameter> parameters = new List<SqlParameter>();
                                foreach (SqlParameter parameter in cmd.Parameters)
                                    if (parameter.Direction == ParameterDirection.Input)
                                        parameters.Add(parameter);
                                for (int i = 0; i < values.Length; i++)
                                    parameters[i].Value = values[i];
                            }

                            cmd.ExecuteNonQuery();
                            con.Close();
                        }
                    }
                }
                catch (SqlException ee)
                {
                    throw new Exception(ee.Message);
                }
            }
            catch (Exception ee)
            {
                throw new Exception(ee.Message);
            }
        }

        private static object ExecuteScalar(System.Data.CommandType commandType, string query, object[] values)
        {
            try
            {
                try
                {
                    using (SqlConnection con = ObtenerCadena())
                    {
                        using (SqlCommand cmd = new SqlCommand(query, con))
                        {
                            // There're three command types: StoredProcedure, Text, TableDirect. The TableDirect   
                            // type is only for OLE DB.    
                            cmd.CommandType = commandType;
                            con.Open();

                            if (values != null && values.Length > 0)
                            {
                                SqlCommandBuilder.DeriveParameters(cmd);
                                List<SqlParameter> parameters = new List<SqlParameter>();
                                foreach (SqlParameter parameter in cmd.Parameters)
                                    if (parameter.Direction == ParameterDirection.Input)
                                        parameters.Add(parameter);
                                for (int i = 0; i < values.Length; i++)
                                    parameters[i].Value = values[i];
                            }
                            
                            var obj = cmd.ExecuteScalar();
                            con.Close();
                            return obj;
                        }
                    }
                }
                catch (SqlException ee)
                {
                    throw new Exception(ee.Message);
                }
            }
            catch (Exception ee)
            {
                throw new Exception(ee.Message);
            }
        }

        private static System.Data.DataSet ExecuteDataset(System.Data.CommandType commandType, string query, object[] values)
        {
            try
            {
                try
                {
                    using (SqlConnection con = ObtenerCadena())
                    {
                        using(SqlDataAdapter adapter = new SqlDataAdapter())
                        {
                            using (SqlCommand cmd = new SqlCommand(query, con))
                            {
                                // There're three command types: StoredProcedure, Text, TableDirect. The TableDirect   
                                // type is only for OLE DB.    
                                cmd.CommandType = commandType;
                                con.Open();
                                if (values != null && values.Length > 0)
                                {
                                    SqlCommandBuilder.DeriveParameters(cmd);
                                    List<SqlParameter> parameters = new List<SqlParameter>();
                                    foreach (SqlParameter parameter in cmd.Parameters)
                                        if (parameter.Direction == ParameterDirection.Input)
                                            parameters.Add(parameter);
                                    for (int i = 0; i < values.Length; i++)
                                        parameters[i].Value = values[i];
                                }

                                adapter.SelectCommand = cmd;
                                System.Data.DataSet ds = new System.Data.DataSet();
                                adapter.Fill(ds);
                                con.Close();
                                return ds;
                            }
                        }
                    }
                }
                catch (SqlException ee)
                {
                    throw new Exception(ee.Message);
                }
            }
            catch (Exception ee)
            {
                throw new Exception(ee.Message);
            }
        }

        #region fetch
        /// <summary>
        /// Obtiene un objeto a partir del los parametros
        /// </summary>
        /// <typeparam name="T">tipo del parametro</typeparam>
        /// <param name="businessObject">Objeto al que se va a mapear</param>
        /// <param name="values">parametros con los que se obtiene el objeto</param>
        /// <returns>xmlreader</returns>
        public static XmlDocument ObjetoFetch<T>(object businessObject, object[] values)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                Type businessObjectType = businessObject.GetType();
                try
                {
                    string[] nmSpace = businessObjectType.FullName.Split('.');
                    var sp = string.Format("{0}.{1}Fetch", nmSpace[2].ToLower(), nmSpace[3]);
                    doc = ExcecuteXmlReader(sp, values);
                }
                catch (SqlException ee)
                {
                    throw new Exception(ee.Message);
                }
                return doc;
            }
            catch (Exception ee)
            {
                throw new Exception(ee.Message);
            }
        }
        #endregion

        #region select de objetos
        public static XmlDocument ObjetoSelect(Type businessObjectType, string spName)
        {
            return ObjetoSelect(businessObjectType, spName, null);
        }
        public static XmlDocument ObjetoSelect(Type businessObjectType, string spName, object[] values)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                try
                {
                    doc = ExcecuteXmlReader(spName, values);
                    return doc;
                }
                catch (SqlException ee)
                {
                    throw new Exception(ee.Message);
                }
            }
            catch (Exception ee)
            {
                throw new Exception(ee.Message);
            }
        }
        public static XmlDocument XMLSelect(string spName, params object[] values)
        {
            try
            {
                XmlDocument reader = new XmlDocument();
                try
                {
                    reader = ExcecuteXmlReader(spName, values);
                    return reader;
                }
                catch (SqlException ee)
                {
                    throw new Exception(ee.Message);
                }
            }
            catch (Exception ee)
            {
                throw new Exception(ee.Message);
            }
        }
        #endregion

        private static SqlConnection ObtenerCadena()
        {
            try
            {
                return new SqlConnection(CadenaDeConexion.ObtenerCadena());
            }
            catch (Exception ee)
            {
                throw new Exception("No se ha podido conectar con el servidor, favor de intentarlo de nuevo", ee.InnerException);
            }
        }


        #region ejecutar sps
        public static void EjecutarQuery(string query, bool esConsulta)
        {
            try
            {
                try
                {
                    ExecuteNonQuery(System.Data.CommandType.Text, query, null);
                }
                catch (SqlException ee)
                {
                    throw new Exception(ee.Message);
                }
            }
            catch (Exception ee)
            {
                throw new Exception(ee.Message);
            }
        }
        public static void EjecutarQuery(string spName)
        {
            EjecutarQuery(spName, null);
        }
        public static void EjecutarQuery(string spName, params object[] pars)
        {
            try
            {
                try
                {
                    ExecuteNonQuery(System.Data.CommandType.StoredProcedure, spName, pars);
                }
                catch (SqlException ee)
                {
                    throw new Exception(ee.Message);
                }
            }
            catch (Exception ee)
            {
                throw new Exception(ee.Message);
            }
        }
        public static object EjecutarScalar(string spName)
        {
            return EjecutarScalar(spName, null);
        }
        public static object EjecutarScalar(string spName, params object[] pars)
        {
            try
            {
                try
                {
                    object obj = ExecuteScalar(System.Data.CommandType.StoredProcedure, spName, pars);
                    return obj;
                }
                catch (SqlException ee)
                {
                    throw new Exception(ee.Message);
                }
            }
            catch (Exception ee)
            {
                throw new Exception(ee.Message);
            }
        }
        public static System.Data.DataSet EjecutarDataset(string spName)
        {
            return EjecutarDataset(spName, null);
        }
        public static System.Data.DataSet EjecutarDataset(string spName, params object[] pars)
        {
            try
            {
                try
                {
                    System.Data.DataSet ds = ExecuteDataset(System.Data.CommandType.StoredProcedure, spName, pars);
                    return ds;
                }
                catch (SqlException ee)
                {
                    throw new Exception(ee.Message);
                }
            }
            catch (Exception ee)
            {
                throw new Exception(ee.Message);
            }
        }
        #endregion

        #region save
        public static void EndTransaction(object tranObj)
        {
            if (tranObj != null)
            {
                SqlTransaction tran = (SqlTransaction)tranObj;
                if (tran.Connection != null)
                {
                    SqlConnection con = tran.Connection;
                    tran.Commit();
                    con.Close();
                }
            }
        }

        public static void DisposeTransaction(object tranObj)
        {
            if (tranObj != null)
            {
                SqlTransaction tran = (SqlTransaction)tranObj;
                if (tran.Connection != null)
                {
                    SqlConnection con = tran.Connection;
                    tran.Rollback();
                    con.Close();
                }
            }
            tranObj = null;
        }

        public static void BeginTransaction(ref object tranObj)
        {
            if (tranObj == null)
            {
                SqlConnection con = ObtenerCadena();
                con.Open();
                tranObj = con.BeginTransaction();
            }
        }

        public static SqlTransaction ObjetoSave(object businessObject, bool _IsDirty, bool _IsForDeleted, bool _IsNew, bool isInTransaction, bool endTran, object tranObj)
        {
            SqlTransaction tran = (SqlTransaction)tranObj;

            try
            {
                if (_IsForDeleted) BusinessObjectDelete(businessObject, isInTransaction, ref tran);
                else if (_IsNew) BusinessObjectInsert(businessObject, isInTransaction, ref tran);
                else if (_IsDirty) BusinessObjectUpdate(businessObject, isInTransaction, ref tran);

                if (tran != null && endTran) { SqlConnection con = tran.Connection; tran.Commit(); if (con != null) con.Close(); }

                return (isInTransaction) ? tran : null;
            }
            catch (SqlException ee)
            {
                if (tran != null) { if (tran.Connection != null) { SqlConnection con = tran.Connection; tran.Rollback(); con.Close(); } }
                throw new Exception(ee.Message);
            }

        }
        private static void BusinessObjectUpdate(object businessObject, bool isInTransaction, ref SqlTransaction tran)
        {
            SqlCommand cmd = null;
            try
            {
                string[] nmSpace = businessObject.GetType().FullName.Split('.');
                var spNAme = string.Format("{0}.{1}Update", nmSpace[2].ToLower(), nmSpace[3]);
                cmd = CreaCommand(spNAme, isInTransaction, ref tran);
                ObjectToSQL.MapperAttributes(cmd, businessObject, 1);
                cmd.ExecuteScalar();
                if (!isInTransaction) cmd.Connection.Close();
            }
            catch (Exception ee)
            {
                if (tran != null) { SqlConnection con = tran.Connection; tran.Rollback(); if (con != null) con.Close(); }
                else if (cmd != null && cmd.Connection != null) { cmd.Connection.Close(); }
                throw new Exception(ee.Message);
            }
        }
        private static void BusinessObjectInsert(object businessObject, bool isInTransaction, ref SqlTransaction tran)
        {
            SqlCommand cmd = null;
            try
            {
                string[] nmSpace = businessObject.GetType().FullName.Split('.');
                var spNAme = string.Format("{0}.{1}Insert", nmSpace[2].ToLower(), nmSpace[3]);
                cmd = CreaCommand(spNAme, isInTransaction, ref tran);
                ObjectToSQL.MapperAttributes(cmd, businessObject, 0);
                cmd.ExecuteScalar();
                if (!isInTransaction) cmd.Connection.Close();

                foreach (SqlParameter par in cmd.Parameters)
                {
                    if (par.Direction == System.Data.ParameterDirection.Output
                        || par.Direction == System.Data.ParameterDirection.InputOutput)
                    {
                        Type tipo = businessObject.GetType();
                        PropertyInfo prop = tipo.GetProperty(par.ParameterName.Replace("@", ""));
                        if (prop != null)
                            prop.SetValue(businessObject, par.Value, null);
                        //break;
                    }
                }
            }
            catch (SqlException ee)
            {
                if (tran != null) { SqlConnection con = tran.Connection; tran.Rollback(); if (con != null) con.Close(); }
                else if (cmd != null && cmd.Connection != null) { cmd.Connection.Close(); }
                throw new Exception(ee.Message);
            }
        }
        private static void BusinessObjectDelete(object businessObject, bool isInTransaction, ref SqlTransaction tran)
        {
            SqlCommand cmd = null;
            try
            {
                string[] nmSpace = businessObject.GetType().FullName.Split('.');
                var spNAme = string.Format("{0}.{1}Delete", nmSpace[2].ToLower(), nmSpace[3]);
                cmd = CreaCommand(spNAme, isInTransaction, ref tran);
                ObjectToSQL.MapperAttributes(cmd, businessObject, 2);
                cmd.ExecuteNonQuery();
                if (!isInTransaction) cmd.Connection.Close();
            }
            catch (Exception ee)
            {
                if (tran != null) { SqlConnection con = tran.Connection; tran.Rollback(); if (con != null) con.Close(); }
                else if (cmd != null && cmd.Connection != null) { cmd.Connection.Close(); }
                throw new Exception(ee.Message);
            }
        }
        private static SqlCommand CreaCommand(string nombreSp, bool isInTransaction, ref SqlTransaction tran)
        {
            try
            {
                SqlConnection con; SqlCommand cmd;
                if (isInTransaction)
                {
                    con = (tran == null) ? ObtenerCadena() : tran.Connection;
                    if (tran == null)
                    {
                        con.Open();
                        tran = con.BeginTransaction();
                    }
                    cmd = new SqlCommand(nombreSp, con, tran);
                }
                else
                {
                    con = ObtenerCadena();
                    con.Open();
                    cmd = new SqlCommand(nombreSp, con);
                }

                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                return cmd;
            }
            catch (SqlException ee)
            {
                throw new Exception(ee.Message);
            }
        }
        #endregion

        #region Utilerias
        public static MethodInfo GetMethod(Type objectType, string method, params object[] parameters)
        {
            MethodInfo result = null;

            // pone todos los parametros en una lista de tipos.
            List<Type> types = new List<Type>();
            if (parameters != null)
                foreach (object item in parameters)
                {
                    if (item == null)
                        types.Add(typeof(object));
                    else
                        types.Add(item.GetType());
                }
            result = FindMethod(objectType, method, types.ToArray());
            return result;
        }
        private static MethodInfo FindMethod(Type objType, string method, Type[] types)
        {
            MethodInfo info = null;
            do
            {
                //encuetra un metodo parecido.
                info = objType.GetMethod(method, types);
                if (info != null)
                    break;

                objType = objType.BaseType;
            } while (objType != null);

            return info;
        }
        #endregion
    }
}
