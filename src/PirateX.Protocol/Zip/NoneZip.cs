﻿namespace PirateX.Protocol
{
    public class NoneZip:IZip
    {
        public byte[] Compress(byte[] datas)
        {
            return datas;
        }

        public byte[] Decompress(byte[] datas)
        {
            return datas; 
        }
    }
}
