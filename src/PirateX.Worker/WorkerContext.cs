using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateX.Worker
{
    public class WorkerContext
    {
        public byte Version { get; set; }

        public string ActionName { get; set; }

        public string SessionId { get; set; }

        public byte[] ClientKeys { get; set; }

        public byte[] ServerKeys { get; set; }

        //public IPirateXRequestInfoBase Request { get; set; }
    }
}
