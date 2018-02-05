using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Newtonsoft.Json;

namespace PirateX.Core
{
    public class DictionaryMapper<TKey,TValue>: SqlMapper.TypeHandler<Dictionary<TKey,TValue>>
    {
        public override void SetValue(IDbDataParameter parameter, Dictionary<TKey, TValue> value)
        {
            parameter.Value = JsonConvert.SerializeObject(value);
        }

        public override Dictionary<TKey, TValue> Parse(object value)
        {
            return JsonConvert.DeserializeObject<Dictionary<TKey, TValue>>(Convert.ToString(value));
        }
    }
}
