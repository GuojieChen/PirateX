using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using Dapper;
using Newtonsoft.Json;

namespace PirateX.Core
{
    public class ArrayJsonMapper<T> : SqlMapper.TypeHandler<T[]>
    {
        public override void SetValue(IDbDataParameter parameter, T[] value)
        {
            parameter.Value = string.Join(",", value);
            //parameter.Value = JsonConvert.SerializeObject(value);
        }

        public override T[] Parse(object value)
        {
            var str = Convert.ToString(value);
            if (string.IsNullOrEmpty(str))
                return new T[0];

            str = str.TrimStart('[').TrimEnd(']');
            if (string.IsNullOrEmpty(str))
                return new T[0];

            return str.Split(new char[] {','})
                    .Select(item => (T)Convert.ChangeType(item, typeof(T))).ToArray()
                ;

            //return JsonConvert.DeserializeObject<T[]>(Convert.ToString(value));
        }
    }

    public class ListJsonMapper<T> : SqlMapper.TypeHandler<List<T>>
    {
        public override void SetValue(IDbDataParameter parameter, List<T> value)
        {
            parameter.Value = string.Join(",", value);

            //parameter.Value = JsonConvert.SerializeObject(value);
        }

        public override List<T> Parse(object value)
        {
            var str = Convert.ToString(value);
            if (string.IsNullOrEmpty(str))
                return new List<T>();

            str = str.TrimStart('[').TrimEnd(']');
            if (string.IsNullOrEmpty(str))
                return new List<T>();

            return str.Split(new char[] { ',' })
                    .Select(item => (T)Convert.ChangeType(item, typeof(T))).ToList()
                ;

            //return JsonConvert.DeserializeObject<List<T>>(Convert.ToString(value));
        }
    }
}
