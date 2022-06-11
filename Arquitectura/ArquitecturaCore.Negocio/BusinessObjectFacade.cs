using System;
using System.Xml;
using ArquitecturaCore.Datos;

namespace ArquitecturaCore.Negocio
{
    public class BusinessObjectFacade
    {
        public static BusinessObjectCollection SelectObjects(Type tipo, string spName)
        {
            return SelectObjects(tipo, spName, null);
        }

        public static BusinessObjectCollection SelectObjects(Type tipo, string spName, params object[] pars)
        {
            try
            {
                BusinessObjectCollection col = new BusinessObjectCollection();
                XmlDocument doc = DataAccess.ObjetoSelect(tipo, spName, pars);
                XMLMapper.XMLToBusinessObjectCollection(doc, col, tipo);
                col.IsDirty = false;
                return col;
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
                DataAccess.EjecutarQuery(spName, pars);
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
                return DataAccess.EjecutarScalar(spName, pars);
            }
            catch (Exception ee)
            {
                throw new Exception(ee.Message);
            }
        }

        public static System.Data.DataSet EjecutarDataset(string spName, params object[] pars)
        {
            try
            {
                return DataAccess.EjecutarDataset(spName, pars);
            }
            catch (Exception ee)
            {
                throw new Exception(ee.Message);
            }
        }

        public static string ConectionStringGet()
        {
            return ArquitecturaCore.Datos.CadenaDeConexion.ObtenerCadena();
        }
    }
}
