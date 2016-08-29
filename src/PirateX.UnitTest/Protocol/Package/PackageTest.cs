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

            var unpackrequestpack = piratepackage.PackRequestPackageToBytes(requestPackage);

            var requestInfo2 = new PirateXRequestInfo(piratepackage.UnPackToRequestPackage(unpackrequestpack));

            Assert.IsNotEmpty(requestInfo.C);
            Assert.IsNotEmpty(requestInfo.Key);

            Assert.IsNotEmpty(requestInfo2.C);


            Assert.AreEqual($"{String.Join("&", requestInfo.Headers.AllKeys.Select(a => a + "=" + requestInfo.Headers[a]))}"
                , $"{String.Join("&", requestInfo2.Headers.AllKeys.Select(a => a + "=" + requestInfo2.Headers[a]))}");

            Assert.AreEqual($"{String.Join("&", requestInfo.QueryString.AllKeys.Select(a => a + "=" + requestInfo.QueryString[a]))}"
                , $"{String.Join("&", requestInfo2.QueryString.AllKeys.Select(a => a + "=" + requestInfo2.QueryString[a]))}");

        }

        [Test]
        public void pack_response_to_bytes_then_unpack_it()
        {
            var piratepack = new ProtocolPackage(new JsonResponseConvert());
            var responseInfo = new PirateXResponseInfo()
            {
                Headers = new NameValueCollection()
                {
                    {"c","test" },
                    { "t","123456"}
                }
            };

            var responsepack = new PirateXResponsePackage()
            {
                HeaderBytes = responseInfo.GetHeaderBytes(),
                ContentBytes = Encoding.UTF8.GetBytes("Hello World!")
            };

            var bytes = piratepack.PackResponsePackageToBytes(responsepack);

            var unpackresponsepack = piratepack.UnPackToResponsePackage(bytes);

            var responseInfo2 = new PirateXResponseInfo(unpackresponsepack.HeaderBytes);

            Console.WriteLine("pack head bytes");
            Console.WriteLine(string.Join(",", responsepack.HeaderBytes));

            Console.WriteLine("unpack head bytes");
            Console.WriteLine(string.Join(",", unpackresponsepack.HeaderBytes));

            Console.WriteLine("pack content bytes");
            Console.WriteLine(string.Join(",", responsepack.ContentBytes));

            Console.WriteLine("unpack content bytes");
            Console.WriteLine(string.Join(",", unpackresponsepack.ContentBytes));


            Assert.IsTrue(responsepack.HeaderBytes.SequenceEqual(unpackresponsepack.HeaderBytes));
            Assert.IsTrue(responsepack.ContentBytes.SequenceEqual(unpackresponsepack.ContentBytes));

            Assert.AreEqual($"{String.Join("&", responseInfo.Headers.AllKeys.Select(a => a + "=" + responseInfo.Headers[a]))}"
                , $"{String.Join("&", responseInfo.Headers.AllKeys.Select(a => a + "=" + responseInfo2.Headers[a]))}");

        }
    }
}
