using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateX.Protocol.Package.ResponseConvert
{
    [DisplayColumn("txt")]
    public class TxtResponseConvert: IResponseConvert
    {
        public byte[] SerializeObject<T>(T t)
        {
            if (t == null)
                return new byte[0];

            return Encoding.UTF8.GetBytes(t.ToString());
        }

        public T DeserializeObject<T>(byte[] datas)
        {
            if (datas == null)
                return default(T);

            return (T)Convert.ChangeType(Encoding.UTF8.GetString(datas),typeof(T));
        }
    }
}
