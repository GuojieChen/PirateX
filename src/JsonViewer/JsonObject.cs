using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace EPocalipse.Json.Viewer
{
    [DebuggerDisplay("Text = {Text}")]
    public class JsonObject
    {
        private readonly JsonFields _fields;
        private JsonObject _parent;
        private string _text;

        public JsonObject()
        {
            _fields=new JsonFields(this);
        }

        public string Id { get; set; }

        public object Value { get; set; }

        public JsonType JsonType { get; set; }

        public JsonObject Parent
        {
            get
            {
                return _parent;
            }
            set
            {
                _parent = value;
            }
        }

        public string Text
        {
            get
            {
                if (_text == null)
                {
                    if (JsonType == JsonType.Value)
                    {
                        string val = Value?.ToString() ?? "<null>";
                        if (Value is string)
                            val = "\"" + val + "\"";
                        _text = $"{Id} : {val}";
                    }
                    else
                        _text = Id;
                }
                return _text;
            }
        }

        public JsonFields Fields => _fields;

        internal void Modified()
        {
            _text = null;
        }

        public bool ContainsFields(params string[] ids )
        {
            return ids.All(s => _fields.ContainId(s));
        }

        public bool ContainsField(string id, JsonType type)
        {
            var field = Fields[id];
            return (field != null && field.JsonType == type);
        }
    }
}
