﻿using System;
using System.ComponentModel;
using System.Reflection;

namespace PirateX.GMSDK.Mapping
{
    public interface IGMUIPropertyMap
    {
        string Name { get;  }

        string DisplayName { get;  }

        string Tips { get;  }

        bool IsRequired { get;  }

        bool CanMulti { get; }

        string GroupName { get; }

        PropertyInfo PropertyInfo { get; set; }

        int OrderId { get; }
    }
    
    /// <summary>
    /// Represents the mapping of a property.
    /// </summary>
    public abstract class GMUIPropertyMap<TGMUIPropertyMap> : IGMUIPropertyMap
    where TGMUIPropertyMap : class, IGMUIPropertyMap 
    {
        public PropertyInfo PropertyInfo { get; set; }

        private string _name;

        public string Name
        {
            get { return string.IsNullOrEmpty(_name) ? PropertyInfo.Name : _name; }
        }

        public TGMUIPropertyMap ToName(string name)
        {
            this._name = name;
            return this as TGMUIPropertyMap;
        }

        public string DisplayName { get; private set; }

        public TGMUIPropertyMap ToDisplayName(string displayName)
        {
            this.DisplayName = displayName;
            return this as TGMUIPropertyMap;
        }

        public string Tips { get; private set; }

        public TGMUIPropertyMap ToTips(string tips)
        {
            this.Tips = tips;
            return this as TGMUIPropertyMap;
        }

        public bool IsRequired { get; private set; }
        
        public TGMUIPropertyMap Required(bool required)
        {
            this.IsRequired = required;
            return this as TGMUIPropertyMap;
        }

        private bool? canMulti; 
        public bool CanMulti {
            get {
                if (canMulti.HasValue)
                    return canMulti.Value;

                return !PropertyInfo.PropertyType.IsPrimitive;
            } }

        public string GroupName { get; private set; }

        public TGMUIPropertyMap ToGroupName(string name)
        {
            this.GroupName = name;
            return this as TGMUIPropertyMap;
        }

        public IGMUIPropertyMap Multi(bool multi)
        {
            canMulti = multi;
            return this as TGMUIPropertyMap;
        }

        public int OrderId { get; private set; }
        public TGMUIPropertyMap ToOrderId(int orderid)
        {
            OrderId = orderid;
            return this as TGMUIPropertyMap;
        }

        #region EditorBrowsableStates
        /// <inheritdoc />
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string ToString()
        {
            return base.ToString();
        }

        /// <inheritdoc />
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        /// <inheritdoc />
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <inheritdoc />
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new Type GetType()
        {
            return base.GetType();
        }
        #endregion
    }

}

