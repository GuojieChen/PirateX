
namespace PirateX.Protocol
{
    public class KeyGenerator
    {
        public int Seed { get; private set; }

        public KeyGenerator(int seed)
        {
            this.Seed = seed;
        }

        /// <summary>
        /// 随机算法
        /// </summary>
        /// <returns></returns>
        public int Rand()
        {
            Seed = Seed * 0x343fd + 0x269EC3;  // a=214013, b=2531011
            return (Seed >> 0x10) & 0x7FFF;
        }

        public byte[] MakeKey()
        {
            const int tableSize = 128;
            var originals = new int[tableSize];

            for (var i = 0; i < tableSize; i++)
            {
                originals[i] = i;
            }

            for (var i = 0; i < tableSize; i++)
            {
                var rand = ((char)(Rand() % tableSize));

                var tmp = originals[i];
                originals[i] = originals[rand];
                originals[rand] = tmp;
            }

            //size [4,16)
            var size = Rand() % 12 + 4;
            var keys = new byte[size];

            for (var i = 0; i < size; i++)
            {
                keys[i] = (byte)originals[i];
            }

            return keys;
        }
    }
}
