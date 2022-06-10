using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.ComponentModel;
using ArquitecturaCore.Datos;
using System.Xml;

namespace ArquitecturaCore.Negocio
{
    [Serializable]
    public struct EstatusAnterior
    {
        public bool isNew;
        public bool isDirty;
        public bool isForDelete;
    }

    [Serializable]
    public class BusinessObject
    {
        #region variables
        private bool _SeEstaCargando = false;
        #endregion 

        #region Propiedades
        private bool _IsDirty = false;
        /// <summary>
        /// Indica si el objeto esta sucio.
        /// </summary>
        [Browsable(false)]
        public bool IsDirty
        {
            get { return _IsDirty; }
            set
            {
                if (_IsDirty != value)
                {
                    _IsDirty = value;
                    OnIsDirtyChange(value);
                }
            }
        }
        /// <summary>
        /// indica si el objeto se esta deserializando
        /// </summary>
        [Browsable(false)]
        public bool SeEstaCargando
        {
            get { return _SeEstaCargando; }
            set { _SeEstaCargando = value; }
        }
        private bool _IsNew = true;
        /// <summary>
        /// Indica si el objeto es nuevo.
        /// </summary>
        [Browsable(false)]
        public bool IsNew
        {
            get { return _IsNew; }
            set { _IsNew = value; }
        }

        private bool _IsForDeleted = false;
        /// <summary>
        /// Indica si el objeto esta marcado para borrarse al guardar.
        /// </summary>
        [Browsable(false)]
        public bool IsForDeleted
        {
            get { return _IsForDeleted; }
            set { _IsForDeleted = value; }
        }

        private string _BrokenRuleMessage = string.Empty;
        /// <summary>
        /// Mensaje que se muestra al usuario sobre las reglas rotas.
        /// </summary>
        [Browsable(false)]
        public string BrokenRuleMessage
        {
            get { return _BrokenRuleMessage; }
            set { _BrokenRuleMessage = value; }
        }

        private BusinessRuleCollection _Reglas = new BusinessRuleCollection();
        /// <summary>
        /// Coleccion de reglas rotas.
        /// </summary>
        protected BusinessRuleCollection Reglas
        {
            get { return _Reglas; }
            set { _Reglas = value; }
        }

        private BusinessObjectCollection _Parent;
        /// <summary>
        /// Coleccion a la que pertenece el objeto.
        /// </summary>
        [Localizable(true)]
        public BusinessObjectCollection Parent
        {
            get { return _Parent; }
            set { _Parent = value; }
        }

        private string _Eliminar = "Eliminar";
        /// <summary>
        /// Propiedad para el binding de los flex
        /// </summary>
        [System.ComponentModel.Localizable(true)]
        public string Eliminar
        {
            get { return _Eliminar; }
            set { _Eliminar = value; }
        }

        private string _Editar = "Editar";
        /// <summary>
        /// Propiedad para el binding de los flex
        /// </summary>
        [System.ComponentModel.Localizable(true)]
        public string Editar
        {
            get { return _Editar; }
            set { _Editar = value; }
        }
        #endregion

        #region Metodos
        /// <summary>
        /// Agrega las reglas rotas, las que no las remueve de la coleccion de reglas.
        /// </summary>
        protected virtual void AgregaReglas()
        {
        }
        /// <summary>
        /// MArca un objeto como para borrarse, lo quita del parent y lo borra de la base de datos.
        /// </summary>
        public void DeleteObject()
        {
            _IsForDeleted = true;
            _IsDirty = true;
            if (!IsNew) Save();
            if (_Parent != null && _Parent.Contains(this))
                _Parent.Remove(this);
        }
        public object DeleteObject(bool isInTransaction, bool endTran, object tran)
        {
            _IsForDeleted = true;
            _IsDirty = true;
            if (!IsNew) tran = Save(isInTransaction, endTran, tran);
            if (_Parent != null && _Parent.Contains(this))
                _Parent.Remove(this);
            return tran;
        }
        /// <summary>
        /// Ensucia el objeto segun el dato que recibe.
        /// </summary>
        /// <param name="isDirty">Estado del objeto.</param>
        public void SetDirty(bool isDirty)
        {
            if (_IsDirty != isDirty && !_SeEstaCargando)
            {
                _IsDirty = isDirty;
                OnIsDirtyChange(_IsDirty);
            }
        }
        /// <summary>
        /// Marca el objeto como sin cambios
        /// </summary>
        public void SetUnchanged()
        {
            SetDirty(false);
            _IsNew =
            _IsForDeleted =
            _SeEstaCargando = false;
        }
        /// <summary>
        /// Marca un objeto como para borrarse.
        /// </summary>
        public void SetForDeleted()
        {
            _IsForDeleted = true;
            SetDirty(true);
        }
        /// <summary>
        /// Marca el objeto como que se esta cargando para que no se ponga dirty ni new
        /// </summary>
        public void SetSeEstaCargando()
        {
            _SeEstaCargando = true;
            _IsDirty =
            _IsNew =
            _IsForDeleted = false;
        }
        /// <summary>
        /// Valida que la coleccion de reglas este vacia y crea el mensaje de reglas rotas para el usuario.
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            AgregaReglas();
            System.Text.StringBuilder mensaje = new StringBuilder();
            foreach (DictionaryEntry llave in _Reglas)
            {
                BusinessRule regla = (BusinessRule)llave.Value;
                if (regla.ReglaRota) mensaje.Insert(0, "\n" + regla.Mensaje);
            }
            _BrokenRuleMessage = (mensaje.Length > 0) ? mensaje.ToString().Substring(1, mensaje.Length - 1) : string.Empty;
            return _Reglas.Count == 0;
        }
        /// <summary>
        /// Marca al objeto como isDirty e IsNew falsos.
        /// </summary>
        public void MarkOld()
        {
            this.IsDirty = false;
            this.IsNew = false;
            _SeEstaCargando = false;
        }
        /// <summary>
        /// Si la cadena nueva es nula le asigna un empty string
        /// valida que la cadena nueva sea diferente a la vieja para 
        /// poderse asignar
        /// </summary>
        /// <param name="oldValue">value de la propiedad</param>
        /// <param name="newValue">variable de la propiedad a la que se va a asignar</param>
        /// <returns>true si se puede asignar</returns>
        public bool AsignaPropiedadString(string oldValue, ref string newValue)
        {
            if (newValue == null) newValue = string.Empty;
            return oldValue.Trim().ToUpper() != newValue.Trim().ToUpper();
        }
        #endregion

        #region IsDirty
        /// <summary>
        /// Se dispara cuando se ensucia el objeto relacionado a la pantalla.
        /// </summary>
        public event EventHandler<IsDirtyChangeArgs> IsDirtyChange;
        /// <summary>
        /// Metodo que dispara el evento de isDirtyChange.
        /// </summary>
        /// <param name="isDirty">Verdadero si esta sucio el objeto, de lo contrario falso.</param>
        public void OnIsDirtyChange(bool isDirty)
        {
            EventHandler<IsDirtyChangeArgs> temp = IsDirtyChange;
            if (temp != null)
                temp(this, new IsDirtyChangeArgs(isDirty));
        }
        #endregion

        #region creacion de eventos
        /// <summary>
        /// Evento cuando se rompe una regla.
        /// </summary>
        public event EventHandler<BrokenRulesArgs> IsBrokenRule;
        /// <summary>
        /// Metodo que se dispara cuando se rompe una regla.
        /// </summary>
        /// <param name="nombre">nombre de la regla rota.</param>
        /// <param name="mensaje">mensaje al usuario.</param>
        public void OnBrokenRule(string nombre, string mensaje)
        {
            EventHandler<BrokenRulesArgs> temp = IsBrokenRule;
            if (temp != null)
                temp(this, new BrokenRulesArgs(nombre, mensaje));
        }
        /// <summary>
        /// Metodo que verifica si la regla se ha roto.
        /// </summary>
        /// <param name="nombre">nombre de la regla.</param>
        /// <param name="mensaje">mensaje de la regla.</param>
        /// <param name="reglaRota">indica si la regla se ha roto.</param>
        /// <returns></returns>
        protected bool CheckRule(string nombre, string mensaje, bool reglaRota)
        {
            if (_SeEstaCargando) return true;
            if (reglaRota)
                OnBrokenRule(nombre, mensaje);
            return !reglaRota;
        }
        /// <summary>
        /// Metodo que verifica si la regla se ha roto.
        /// </summary>
        /// <param name="mensaje">mensaje de la regla.</param>
        /// <param name="reglaRota">indica si la regla se ha roto.</param>
        /// <returns></returns>
        protected bool CheckRule(string mensaje, bool reglaRota)
        {
            return CheckRule(string.Empty, mensaje, reglaRota);
        }
        #endregion

        #region Fetch
        public static T Fetch<T>(params object[] values)
        {
            Type tipo = typeof(T);
            T objeto = (T)Activator.CreateInstance<T>();
            XmlDocument doc = DataAccess.ObjetoFetch<T>(objeto, values);
            XMLMapper.XMLToBusinessObject(doc, objeto);
            return objeto;
        }
        #endregion

        #region estatus Anterior
        private EstatusAnterior _OldEstatus;
        public virtual void RollBackStatus()
        {
            if (_OldEstatus.isDirty) SetDirty(true);
            _IsForDeleted = _OldEstatus.isForDelete;
            _IsNew = _OldEstatus.isNew;
        }
        #endregion

        #region Save
        public virtual void Save()
        {
            Save(false, false, null);
        }
        public static void EndTransaction(object tranObj)
        {
            ArquitecturaCore.Datos.DataAccess.EndTransaction(tranObj);
        }
        public virtual object Save(bool isIntransaction, bool endTran, object tran)
        {
            try
            {
                #region actualiza el estatus anterior que tenia antes de guardar el registro
                _OldEstatus.isDirty = _IsDirty;
                _OldEstatus.isForDelete = _IsForDeleted;
                _OldEstatus.isNew = _IsNew;
                #endregion

                // si el objeto es valido o es para borrarse realiza el gaurdado
                if (IsValid() || _IsForDeleted)
                {
                    // guarda el objeto y regresa la transaccion (null en caso de no guardar bajo transaccion)
                    tran = DataAccess.ObjetoSave(this, _IsDirty, _IsForDeleted, _IsNew, isIntransaction, endTran, tran);
                    _Clone = null;
                    // actualiza el estatus del objeto
                    this.IsDirty = false;
                    this.IsNew = false;
                    // si se borro el objeto lo remueve del parent
                    if (this.IsForDeleted && _Parent != null && _Parent.Contains(this)) _Parent.Remove(this);
                }
                // si no es valido muestra el mensaje
                else throw new Exception(BrokenRuleMessage);
                return tran;
            }
            catch (Exception ee)
            {
                // termina la transaccion si existia una
                ArquitecturaCore.Datos.DataAccess.DisposeTransaction(tran);
                // regresa el objeto al estatus anterior al intento de guardar
                // son el isforDelete, isDirty e isNew
                RollBackStatus();
                // cambia los mensajes de error por mensajes mas amigables para el usuario
                string msj = ee.Message;
                if (ee.Message.Contains("UNIQUE KEY"))
                {
                    int ini = ee.Message.IndexOf("_IX_") + 4;
                    int fin = ee.Message.IndexOf("'", ini);
                    msj = msj.Substring(ini, fin - ini);
                    msj = "El valor que intenta asignar al campo " + msj + " ya se encuentra asignado en otro registro.";
                }
                else if (ee.Message.Contains("PRIMARY KEY"))
                {
                    int ini = ee.Message.IndexOf("_PK_") + 4;
                    int fin = ee.Message.IndexOf("'", ini);
                    msj = msj.Substring(ini, fin - ini);
                    msj = "El valor que intenta asignar al campo " + msj + " ya se encuentra asignado en otro registro.";
                }
                else if (ee.Message.Contains("FK_"))
                {
                    if (!this.IsNew)
                    {
                        msj = "No se puede borrar el registro debido a que está siendo utilizado por otros procesos.";
                        _IsForDeleted = false;
                    }
                    else msj = "Error al guardar, favor de intentar nuevamente";
                }
                // lanza el error
                throw new Exception(msj);
            }
        }
        #endregion

        internal void AgregaEventosColOnLoad(BusinessObjectCollection col)
        {
            if (col.Count == 1)
                col.IsBrokenRule += new EventHandler<BrokenRulesArgs>(col_IsBrokenRule);
        }

        void col_IsBrokenRule(object sender, BrokenRulesArgs e)
        {
            CheckRule(e.Mensaje, true);
        }

        public BusinessObject Clone()
        {
            Type tipo = this.GetType();

            BusinessObject clone = (BusinessObject)Activator.CreateInstance(tipo);

            System.Reflection.PropertyInfo[] infos = tipo.GetProperties();
            foreach (System.Reflection.PropertyInfo info in infos)
            {
                if (info.CanRead && info.CanWrite)
                {
                    object value = info.GetValue(this, null);
                    if (info.PropertyType.Name == "BusinessObjectCollection" && value != null && info.Name != "Parent")
                    {
                        BusinessObjectCollection coleccion = new BusinessObjectCollection();

                        foreach (BusinessObject child in (BusinessObjectCollection)value)
                        {
                            BusinessObject cloneChild = child.Clone();
                            coleccion.Add(cloneChild);
                        }

                        info.SetValue(clone, coleccion, null);
                    }
                    else if ((info.PropertyType.Name == "BusinessObject" || info.PropertyType.BaseType.Name == "BusinessObject") && value != null)
                    {
                        BusinessObject cloneValue = ((BusinessObject)value).Clone();
                        info.SetValue(clone, cloneValue, null);
                    }
                    else info.SetValue(clone, value, null);
                }
            }

            return clone;
        }

        #region clone
        private object _Clone;

        public void BackUp()
        {
            if (!IsNew)
            {
                Type tipo = this.GetType();

                if (_Clone == null)
                    _Clone = Activator.CreateInstance(tipo);

                System.Reflection.PropertyInfo[] infos = tipo.GetProperties();
                foreach (System.Reflection.PropertyInfo info in infos)
                {
                    if (info.CanRead && info.CanWrite)
                    {
                        object value = info.GetValue(this, null);
                        if (info.PropertyType.Name == "BusinessObjectCollection" && value != null && info.Name != "Parent")
                        {
                            foreach (BusinessObject child in (BusinessObjectCollection)value)
                                child.BackUp();
                        }
                        else if ((info.PropertyType.Name == "BusinessObject" || info.PropertyType.BaseType.Name == "BusinessObject") && value != null)
                        {
                            ((BusinessObject)value).BackUp();
                        }
                        info.SetValue(_Clone, value, null);
                    }
                }
            }
        }

        public void Undo()
        {
            if (_Clone != null)
            {
                Type tipo = this.GetType();
                System.Reflection.PropertyInfo[] infos = tipo.GetProperties();

                foreach (System.Reflection.PropertyInfo info in infos)
                {
                    if (info.CanRead && info.CanWrite)
                    {
                        object value = info.GetValue(_Clone, null);
                        if (info.PropertyType.Name == "BusinessObjectCollection" && value != null && info.Name != "Parent")
                        {
                            foreach (BusinessObject child in (BusinessObjectCollection)value)
                                child.Undo();
                        }
                        else if ((info.PropertyType.Name == "BusinessObject" || info.PropertyType.BaseType.Name == "BusinessObject") && value != null)
                        {
                            ((BusinessObject)value).Undo();
                        }
                        info.SetValue(this, value, null);
                    }
                }
            }
        }
        #endregion
    }

    /// <summary>
    /// Clase de argumentos para el evento isdirtyChange.
    /// </summary>
    public class IsDirtyChangeArgs : EventArgs
    {
        #region Declaracion de variables
        private bool _IsDirty = false;
        #endregion

        #region Constructor
        public IsDirtyChangeArgs(bool isDirty)
        {
            _IsDirty = isDirty;
        }
        #endregion

        #region Propiedades
        public bool IsDirty
        {
            get { return _IsDirty; }
            set { _IsDirty = value; }
        }
        #endregion
    }

    /// <summary>
    /// Clase de argumentos para el evento onbrookenrules.
    /// </summary>
    public class BrokenRulesArgs : EventArgs
    {
        #region Declaracion de variables
        private string _Nombre = string.Empty;
        private string _Mensaje = string.Empty;
        #endregion

        #region Constructor
        public BrokenRulesArgs(string nombre, string mensaje)
        {
            _Nombre = nombre;
            _Mensaje = mensaje;
        }
        #endregion

        #region Propiedades
        /// <summary>
        /// Nombre de la regla.
        /// </summary>
        public string Nombre
        {
            get { return _Nombre; }
            set { _Nombre = value; }
        }
        /// <summary>
        /// Mensaje de la regla rota.
        /// </summary>
        public string Mensaje
        {
            get { return _Mensaje; }
            set { _Mensaje = value; }
        }
        #endregion
    }
}
