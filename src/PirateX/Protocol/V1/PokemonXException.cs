using System;
using PirateX.GException;

namespace PirateX.Protocol.V1
{
    [Serializable]
    public class PokemonXException : AbstactGameException
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
