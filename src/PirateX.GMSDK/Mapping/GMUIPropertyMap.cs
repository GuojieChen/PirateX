using System;
using System.ComponentModel;
using System.Reflection;

namespace PirateX.GMSDK.Mapping
{
    public interface IGMUIPropertyMap
    {
        string Control { get; }

        string Name { get;  }

        string DisplayName { get;  }

        string Tips { get;  }

        bool IsRequired { get;  }

        string GroupName { get; }

        string DevaultValue { get; set; }

        PropertyInfo PropertyInfo { get; set; }

        int OrderId { get; }

        Func<string, string> ValidateFunc { get; }

        void Validate(Func<string, string> func);
    }
    
    /// <summary>
    /// Represents the mapping of a property.
    /// </summary>
    public abstract class GMUIPropertyMap<TGMUIPropertyMap> : IGMUIPropertyMap
    where TGMUIPropertyMap : class, IGMUIPropertyMap 
    {
        public abstract string Control { get; }

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

        public string GroupName { get; private set; }
        public TGMUIPropertyMap ToGroupName(string name)
        {
            this.GroupName = name;
            return this as TGMUIPropertyMap;
        }

        public int OrderId { get; private set; }
        public TGMUIPropertyMap ToOrderId(int orderid)
        {
            OrderId = orderid;
            return this as TGMUIPropertyMap;
        }

        public Func<string, string> ValidateFunc { get; set; }

        public void Validate(Func<string, string> func)
        {
            ValidateFunc = func;
        }

        public string DevaultValue { get; set; }
        public TGMUIPropertyMap ToDevaultValue(object value)
        {
            DevaultValue = Convert.ToString(value);
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

