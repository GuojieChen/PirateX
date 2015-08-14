using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Core.Ex
{
    public interface IGameError<Key> :IGameError
    {
        Key Code { get; set; }


    }

    public interface IGameError
    {
        
    }
}
