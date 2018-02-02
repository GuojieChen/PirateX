using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateX.Protocol
{
    public class KeyGenerator
    {
        /// <summary>
        /// 随机算法
        /// </summary>
        /// <returns></returns>
        private static int Rand(int seed)
        {
            seed = seed * 0x343fd + 0x269EC3;  // a=214013, b=2531011
            return (seed >> 0x10) & 0x7FFF;
        }

        public static byte[] MakeKey(int seed)
        {
            const int tableSize = 128;
            var originals = new int[tableSize];

            for (var i = 0; i < tableSize; i++)
            {
                originals[i] = i;
            }

            for (var i = 0; i < tableSize; i++)
            {
                var rand = ((char)(Rand(seed) % tableSize));

                var tmp = originals[i];
                originals[i] = originals[rand];
                originals[rand] = tmp;
            }

            //size [4,16)
            var size = Rand(seed) % 12 + 4;
            var keys = new byte[size];

            for (var i = 0; i < size; i++)
            {
                keys[i] = (byte)originals[i];
            }

            return keys;
        }
    }
}
