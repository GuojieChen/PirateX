using System;
using PirateX.GException;

namespace PirateX.Protocol.V1
{
    [Serializable]
    public class PokemonXException : GameException
    {
        public PokemonXException(Enum code) : base(code)
        {
        }

        public PokemonXException(Enum code, params object[] strs) : base(code, strs)
        {
        }
    }
}
