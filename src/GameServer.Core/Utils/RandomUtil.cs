using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Core.Utils
{
    public class RandomUtil
    {
        private static Random _random;

        public static Random Random
        {
            get
            {
                if (_random == null)
                {
                    var tick = DateTime.Now.Ticks;
                    _random = new Random((int)(tick & 0xffffffffL) | (int)(tick >> 32));
                }
                return _random;
            }
        }
    }
}
