using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateX.Protocol.Package
{
    public interface IPirateXPackage
    {

        byte[] HeaderBytes { get; set; }

        byte[] ContentBytes { get; set; }
    }
}
