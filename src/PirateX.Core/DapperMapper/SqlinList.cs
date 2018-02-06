using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateX.Core
{
    public class SqlinList<T>:List<T>
    {
        public SqlinList(IEnumerable<T> s):base(s)
        {

        }
    }
}
