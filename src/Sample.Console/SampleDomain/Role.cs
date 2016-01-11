using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PirateX.Core.Domain.Entity;

namespace GameServer.Console.SampleDomain
{
    public class Role:IEntity<int>
    {
        public int Id { get; set; }

        public DateTime CreateAt { get; set; }
    }
}
