﻿using System;
using GameServer.Core.GException;

namespace GameServer.Core.Protocol.V1
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
