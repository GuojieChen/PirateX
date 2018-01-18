using System;
using System.Collections.Generic;

namespace PirateX.Core.Utils
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

        public static List<int> RandomList(int all, int part)
        {
            var list = new List<int>();
            while (true)
            {
                int n = Random.Next(0, all);
                if (!list.Contains(n))
                {
                    list.Add(n);
                    if (list.Count == part)
                    {
                        break;
                    }
                }
            }
            return list;
        }
    }
}
