using System;
using System.Collections.Generic;
using System.Data;
using Dapper;
using Newtonsoft.Json;

namespace PirateX.Core
{
    public class ArrayJsonMapper<T> : SqlMapper.TypeHandler<T[]>
    {
        public override void SetValue(IDbDataParameter parameter, T[] value)
        {
            parameter.Value = JsonConvert.SerializeObject(value);
        }

        public override T[] Parse(object value)
        {
            return JsonConvert.DeserializeObject<T[]>(Convert.ToString(value));
        }
    }

    public class ListJsonMapper<T> : SqlMapper.TypeHandler<List<T>>
    {
        public override void SetValue(IDbDataParameter parameter, List<T> value)
        {
            parameter.Value = JsonConvert.SerializeObject(value);
        }

        public override List<T> Parse(object value)
        {
            return JsonConvert.DeserializeObject<List<T>>(Convert.ToString(value));
        }
    }
}
