using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PirateX.Client;

namespace Sample.Clinet
{
    public class Program
    {

        public static void Main(string[] args)
        {
            var client = new PirateXClient("ps://localhost:4012");

            Console.WriteLine("Press any key to start!");
            Console.Read();

            client.Open();


            Console.WriteLine("OK");
            Console.Read();
            Console.Read();
        }
    }
}
