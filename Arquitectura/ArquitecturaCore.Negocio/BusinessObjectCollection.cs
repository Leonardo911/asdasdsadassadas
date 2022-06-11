using System;
using System.Collections;
using System.ComponentModel;
using System.Collections.Generic;
using System.Reflection;

namespace ArquitecturaCore.Negocio
{
    [Serializable]
    public class BusinessObjectCollection : CollectionBase, ITypedList, IEnumerable
    {
        #region Constructor
        public BusinessObjectCollection()
        {
        }
        #endregion

        public BusinessObject this[int index]
        {
            get { return (BusinessObject)List[index]; }
            set { List[index] = value; }
        }

        protected override void OnValidate(object value)
        {
            base.OnValidate(value);
        }

        #region Variables
        private bool _IsDirty = false;
        #endregion

        #region Propiedades
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

        private object _Tag = null;
        /// <summary>
        /// puede almacenar cualquier dato
        /// </summary>
        public object Tag
        {
            get { return _Tag; }
            set { _Tag = value; }
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

        #region Reglas
        public event EventHandler<BrokenRulesArgs> IsBrokenRule;

        public void OnBrokenRule(string nombre, string mensaje)
        {
            EventHandler<BrokenRulesArgs> temp = IsBrokenRule;
            if (temp != null)
                temp(this, new BrokenRulesArgs(nombre, mensaje));
        }
        #endregion

        #region Eventos sobreescritos
        protected override void OnInsert(int index, object value)
        {
            base.OnInsert(index, value);
            if (value is BusinessObject)
            {
                BusinessObject objeto = (BusinessObject)value;
                objeto.IsDirtyChange += new EventHandler<IsDirtyChangeArgs>(objeto_IsDirtyChange);
                objeto.IsBrokenRule += new EventHandler<BrokenRulesArgs>(objeto_IsBrookenRule);
            }
        }

        void objeto_IsBrookenRule(object sender, BrokenRulesArgs e)
        {
            OnBrokenRule(e.Nombre, e.Mensaje);
        }
        #endregion

        #region Eventos
        void objeto_IsDirtyChange(object sender, IsDirtyChangeArgs e)
        {
            if (!_IsDirty) IsDirty = e.IsDirty;
        }
        #endregion

        #region Metodos
        public int Add(BusinessObject value)
        {
            if (!Contains(value))
            {
                value.Parent = this;
                return List.Add(value);
            }
            return -1;
        }

        public int Add(BusinessObject value, bool setDirty)
        {
            if (setDirty) IsDirty = true;
            return Add(value);
        }

        public void Insert(int index, BusinessObject value)
        {
            List.Insert(index, value);
        }

        public void Remove(BusinessObject value)
        {
            List.Remove(value);
        }

        public bool Contains(BusinessObject value)
        {
            return List.Contains(value);
        }

        public void Save()
        {
            foreach (BusinessObject obj in this)
                obj.Save();
            this.IsDirty = false;
        }

        public object Save(bool isInTransaction, bool endTransaction, object tran)
        {
            try
            {
                for (int i = 0; i < Count; i++)
                {
                    BusinessObject obj = (BusinessObject)this[i];
                    // si el objeto termina la transaccion la termina con el ultimo elemento
                    if (i == Count - 1) tran = obj.Save(isInTransaction, endTransaction, tran);
                    else tran = obj.Save(isInTransaction, false, tran);
                }
                IsDirty = false;
                return tran;
            }
            catch (Exception ee)
            {
                throw new Exception(ee.Message);
            }
        }

        public int IndexAt(BusinessObject obj)
        {
            for (int i = 0; i < this.Count; i++)
            {
                BusinessObject ob = (BusinessObject)this[i];
                if (ob == obj) return i;
            }
            return -1;
        }

        public ArrayList ToArrayList()
        {
            ArrayList list = new ArrayList();
            foreach (BusinessObject bo in this)
                list.Add(bo);
            return list;
        }
        #endregion

        #region ITypedList Members

        public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
        {
            PropertyDescriptorCollection pdc = null;

            if (listAccessors == null)
            {
                pdc = (this.Count > 0)
                    ? TypeDescriptor.GetProperties(this[0].GetType())
                    : TypeDescriptor.GetProperties(typeof(BusinessObject));
            }
            else pdc = TypeDescriptor.GetProperties(this[0].GetType());
            return pdc;
        }

        public string GetListName(PropertyDescriptor[] listAccessors)
        {
            return typeof(BusinessObject).Name;
        }
        #endregion
    }
}
