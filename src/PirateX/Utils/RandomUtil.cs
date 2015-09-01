using System;

namespace PirateX.Utils
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
