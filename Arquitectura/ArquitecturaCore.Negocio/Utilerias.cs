using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Reflection;
using System.Text;

namespace ArquitecturaCore.Negocio
{
    public class Utilerias
    {
        /// <summary>
        /// obtiene el elemento maximo de un arreglo de decimales
        /// </summary>
        public static decimal EncuentraMax(System.Collections.ArrayList arr)
        {
            decimal max = 0;
            if (arr.Count > 0)
            {
                foreach (decimal element in arr)
                    if (element > max) max = element;
            }
            return max;
        }
        /// <summary>
        /// Obtiene el elemento menor de un arreglo de decimales
        /// </summary>
        public static decimal EncuentraMin(System.Collections.ArrayList arr)
        {
            if (arr.Count > 0)
            {
                decimal min = (decimal)arr[0];
                foreach (decimal element in arr)
                    if (min > element) min = element;
                return min;
            }
            return decimal.Zero;
        }
        /// <summary>
        /// Ordena una coleccion por la propiedad que recibe
        /// </summary>
        /// <param name="col">coleccion a ordenar</param>
        /// <param name="propiedadStr">propiedad a ordenar</param>
        /// <returns></returns>
        public static System.Collections.ArrayList OrdenaColeccion(System.Collections.ArrayList col, string propiedadStr)
        {
            #region Ordena la coleccion mediante el modelo shell
            int salto = col.Count / 2;
            while (salto > 0)
            {
                bool huboIntercambios = true;
                while (huboIntercambios)
                {
                    huboIntercambios = false;
                    for (int i = 0; i + salto < col.Count; i++)
                    {
                        // saca los objetos que se van a comparar.
                        BusinessObject temp1 = (BusinessObject)col[i], temp2 = (BusinessObject)col[i + salto];

                        Type tipo = temp1.GetType();
                        System.Reflection.PropertyInfo propiedad = tipo.GetProperty(propiedadStr);
                        int str1 = (int)propiedad.GetValue(temp1, null);
                        int str2 = (int)propiedad.GetValue(temp2, null);
                        // si el rank es menor, reordena la coleccion.
                        if (str2 < str1)
                        {
                            col[i] = temp2;
                            col[i + salto] = temp1;
                            // marca que hubo al menos un intercambio.
                            huboIntercambios = true;
                        }
                    }
                }
                // si no hubo intercambios asigna el nuevo valor al salto.
                salto = salto / 2;
            }
            #endregion

            return col;
        }
        public static System.Drawing.Image BytesAImagen(byte[] imgByte)
        {
            try
            {
                if (imgByte != null)
                {
                    System.IO.MemoryStream ms = new System.IO.MemoryStream(imgByte);
                    return System.Drawing.Bitmap.FromStream(ms);
                }
                else return null;
            }
            catch (Exception ee)
            {
                throw new Exception(ee.Message);
            }
        }
        public static byte[] StreamABytes(System.IO.Stream str)
        {
            try
            {
                if (str != null)
                {
                    System.IO.MemoryStream ms = new System.IO.MemoryStream();
                    System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(str);
                    bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    return ms.ToArray();
                }
                else return null;
            }
            catch (Exception ee)
            {
                throw new Exception(ee.Message);
            }
        }

        public static byte[] StreamABytes(System.IO.Stream str, int height, int width, bool preservRatio)
        {
            try
            {
                if (str != null)
                {
                    System.IO.MemoryStream ms = new System.IO.MemoryStream();
                    System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(str);
                    if (preservRatio)
                    {
                        double ratio = (double)bmp.Height / (double)bmp.Width;
                        double newwidth = height / ratio;
                        width = (int)newwidth;
                    }
                    System.Drawing.Bitmap bmpResize = new System.Drawing.Bitmap(bmp, width, height);
                    bmpResize.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    return ms.ToArray();
                }
                else return null;
            }
            catch (Exception ee)
            {
                throw new Exception(ee.Message);
            }
        }

        public static string EncriptarCadena(string cadena)
        {
            System.Security.Cryptography.HashAlgorithm hashValue = new System.Security.Cryptography.SHA1CryptoServiceProvider();
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(cadena + "autornado");
            byte[] byteHash = hashValue.ComputeHash(bytes);
            hashValue.Clear();
            return (Convert.ToBase64String(byteHash));
        }

        public static void EndTransaction(object tranObj)
        {
            ArquitecturaCore.Datos.DataAccess.EndTransaction(tranObj);
        }

        public static void EjecutarConsultaSQL(string query)
        {
            ArquitecturaCore.Datos.DataAccess.EjecutarQuery(query, true);
        }

        public static Enum[] EnumToArray(Type tipo)
        {


            //get the public static fields (members of the enum)
            System.Reflection.FieldInfo[] fi = tipo.GetFields(BindingFlags.Static | BindingFlags.Public);

            //create a new enum array
            Enum[] values = new Enum[fi.Length];
            var enumeration = Activator.CreateInstance(tipo);
            //populate with the values
            for (int iEnum = 0; iEnum < fi.Length; iEnum++)
            {

                values[iEnum] = (Enum)fi[iEnum].GetValue(enumeration);
            }
            //return the array
            return values;
        }

        public static DateTime GetDateTimeServer()
        {
            DateTime dt = (DateTime)ArquitecturaCore.Negocio.BusinessObjectFacade.EjecutarScalar("GetDateTimeServer");
            return dt;
        }

        /// <summary>
        /// regresa el factorial del numero dado
        /// </summary>
        /// <param name="numero">numero que se desea saber su factorial</param>
        /// <returns>factorial del numero mandado</returns>
        public static int Factorial(int numero)
        {
            int factorial = 1;
            numero = Math.Abs(numero);
            for (int i = 2; i <= numero; i++)
                factorial = Math.Abs(factorial * i);
            return factorial;
        }
    }
}
