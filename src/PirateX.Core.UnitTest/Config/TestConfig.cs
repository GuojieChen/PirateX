﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateX.Core.UnitTest.Config
{
    [ConfigIndex("Lv")]
    [ConfigIndex(true,"Lv","Id")]
    [ConfigIndex("Lv", "Atk")]
    public class TestConfig:IConfigIdEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Lv { get; set; }

        public int Atk { get; set;}

        public int Def { get; set; }


        public override string ToString()
        {
            return $"{Id}\t{Lv}\t{Name}\t{Atk}\t{Def}";
        }
    }


    public class KeyValueTestConfig :IConfigKeyValueEntity
    {
        public string Id { get; set; }
        public string Value { get; set; }

        public override string ToString()
        {
            return $"{Id}\t{Value}";
        }
    }
}
