using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateX.GMSDK
{
    public interface IGMUICheckedDataProvider
    {
        IEnumerable<GMUIDataCheckbox> GetCheckedItems();
    }
}
