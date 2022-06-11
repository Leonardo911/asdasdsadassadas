using System;
using System.Collections.Generic;
using System.Text;

namespace ArquitecturaCore.Negocio
{
    public class Encriptador
    {
        #region variables
        private static string _PatronDeBusqueda = "QPWOEIRUTYqpwoeiruty1029384756 ALSKDJFHGalskdjfhgZMXNCBVzmxncbv";
        private static string _PatronEncripta = "YTURIEOWPQGFHDJSKALZMXNCB V6574839201qazwsxedcrfvtbgynhumjiklop";
        #endregion

        #region Encriptador de cadena
        public static string EncriptarCadena(string cadena)
        {
            int idx = -1;
            string result = string.Empty;

            for (idx = 0; idx < cadena.Length; idx++)
                result += EncriptarCaracter(cadena.Substring(idx, 1), cadena.Length, idx);
            return result;
        }
        private static string EncriptarCaracter(string caracter, int variable, int a_indice)
        {
            string caracterEncriptado = string.Empty;
            int indice = -1;
            if (_PatronDeBusqueda.IndexOf(caracter) != -1)
            {
                indice = (_PatronDeBusqueda.IndexOf(caracter) + variable + a_indice) % _PatronDeBusqueda.Length;
                return _PatronEncripta.Substring(indice, 1);
            }
            return caracter;
        }
        #endregion

        #region Desencriptador
        public static string DesencriptaCadena(string cadena)
        {
            int idx = 0;
            string result = string.Empty;
            for (idx = 0; idx < cadena.Length; idx++)
                result += DesencriptaCaracter(cadena.Substring(idx, 1), cadena.Length, idx);
            return result;
        }
        private static string DesencriptaCaracter(string caracter, int variable, int a_indice)
        {
            int indice = 0;
            if (_PatronEncripta.IndexOf(caracter) != -1)
            {
                if (_PatronEncripta.IndexOf(caracter) - variable - a_indice > 0)
                    indice = (_PatronEncripta.IndexOf(caracter) - variable - a_indice) & _PatronEncripta.Length;
                else indice = _PatronDeBusqueda.Length + ((_PatronEncripta.IndexOf(caracter) - variable - a_indice) % _PatronEncripta.Length);
                indice = indice % _PatronEncripta.Length;
                string car = _PatronDeBusqueda.Substring(indice, 1);
                return car;
            }
            else return caracter;
        }
        #endregion
    }
}
