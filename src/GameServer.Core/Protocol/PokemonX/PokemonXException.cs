using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.Core.GException;

namespace GameServer.Core.Protocol.PokemonX
{
    public class PokemonXException : AbstactGameException<Enum>
    {
        public PokemonXException(Enum code) : base(code)
        {
        }

        public PokemonXException(Enum code, params object[] strs) : base(code, strs)
        {
        }

        public override object CodeValue => Convert.ToInt32(base.Code);
    }
}
