using System;
using System.Collections.Generic;
using System.Text;

namespace ArquitecturaCore.Datos
{
    public class CadenaDeConexion
    {
        public static string ObtenerCadena()
        {
            // obtiene la cadena de conexion de el app.config del proyecto
            //string cadena = @"Data Source=50.63.184.5\SQLEXPRESS;Initial Catalog=siat;User ID=pdv123; Password=admin123";
            string cadena = @"Data Source=wpfdeveloper.com;Initial Catalog=NavojoaDigital;User ID=navojoa; Password=NavojoaDigital2022";
            // string cadena = @"Data Source=SHADOWBLAST\SQLEXPRESS;Initial Catalog=siat;Integrated Security=True";
            return cadena;
        }
    }
}
