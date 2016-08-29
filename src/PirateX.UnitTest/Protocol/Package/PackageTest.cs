using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using NUnit.Framework;
using PirateX.Protocol;
using PirateX.Protocol.Package;
using ServiceStack;

namespace PirateX.UnitTest.Protocol.Package
{
    [TestFixture]
    public class PackageTest
    {
        [Test]
        public void pack_request_to_bytes_then_unpack_it()
        {
            var piratepackage = new ProtocolPackage(new JsonResponseConvert());

            var requestInfo = new PirateXRequestInfo(new NameValueCollection()
            {
                {"c","test" },
                { "t","123456"}
            },new NameValueCollection()
            {
                 {"p1","1" },
                 {"p2","2" },
            });

            var requestPackage = new PirateXRequestPackage()
            {
                HeaderBytes = Encoding.UTF8.GetBytes($"{String.Join("&", requestInfo.Headers.AllKeys.Select(a => a + "=" + requestInfo.Headers[a]))}"),
                ContentBytes = Encoding.UTF8.GetBytes($"{String.Join("&", requestInfo.QueryString.AllKeys.Select(a => a + "=" + requestInfo.QueryString[a]))}")
            };

            var bytes = piratepackage.PackRequestPackageToBytes(requestPackage);
            var requestInfo2 = new PirateXRequestInfo(piratepackage.UnPackToRequestPackage(bytes));

            Assert.IsNotEmpty(requestInfo.C);
            Assert.IsNotEmpty(requestInfo.Key);

            Assert.IsNotEmpty(requestInfo2.C);


            Assert.AreEqual($"{String.Join("&", requestInfo.Headers.AllKeys.Select(a => a + "=" + requestInfo.Headers[a]))}"
                , $"{String.Join("&", requestInfo2.Headers.AllKeys.Select(a => a + "=" + requestInfo2.Headers[a]))}");

            Assert.AreEqual($"{String.Join("&", requestInfo.QueryString.AllKeys.Select(a => a + "=" + requestInfo.QueryString[a]))}"
                , $"{String.Join("&", requestInfo2.QueryString.AllKeys.Select(a => a + "=" + requestInfo2.QueryString[a]))}");

        }
    }
}
