using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using PirateX.Core.Utils;

namespace PirateX.Core.UnitTest.Utils
{
    [TestFixture]
    public class DateTimeTest
    {
        [Test]
        public void test_ToDateTime()
        {
            var datetime = new DateTime(2018,1,1,10,10,11);
            var datetimeStr = datetime.FromDateTime();
            Console.WriteLine(datetimeStr);
            var toDateTime = datetimeStr.ToDateTime();
            Console.WriteLine(toDateTime);
            Console.WriteLine(toDateTime.Kind);

        }
    }
}
