using Dapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateX.Core
{
    public class ObjectMapper<TObject> : SqlMapper.TypeHandler<TObject>
    {
        public override TObject Parse(object value)
        {
            if (value == null)
                return default(TObject);


            return JsonConvert.DeserializeObject<TObject>(Convert.ToString(value));
        }

        public override void SetValue(IDbDataParameter parameter, TObject value)
        {
            parameter.Value = JsonConvert.SerializeObject(value);
        }
    }
}
