using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Console
{
    public class Test
    {

    }

    public class T1
    {
        public int V { get; set; }
    }


    public class A
    {
        public static int V { get; protected set; } 

        public A()
        {
            V++;
            System.Console.WriteLine("A");
        }

    }

    public class B:A
    {
        public B()
        {
            V++;
            System.Console.WriteLine("B");
        }

        public void Do(T1 t)
        {
            t.V++;
        }
    }
}
