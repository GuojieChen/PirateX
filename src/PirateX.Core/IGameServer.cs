﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateX.Core
{
    public interface IGameServer
    {
        IServerContainer ServerContainer { get; set; }
    }
}
