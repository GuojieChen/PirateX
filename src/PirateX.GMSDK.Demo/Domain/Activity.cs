using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PirateX.Middleware;

namespace PirateX.GMSDK.Demo
{
    public class Activity:IActivity
    {
        public int Id { get; set; }
        public int Did { get; set; }
        public string Name { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public int Type { get; set; }
        public int[] Days { get; set; }
        public string Args { get; set; }
        public bool IsSend { get; set; }
        public string Remark { get; set; }
    }
}
