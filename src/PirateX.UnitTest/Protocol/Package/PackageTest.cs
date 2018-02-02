using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using NUnit.Framework;
using PirateX.Client.Crypto;
using PirateX.Protocol;
using ServiceStack;

namespace PirateX.UnitTest.Protocol.Package
{
    [TestFixture]
    public class PackageTest
    {
        [Test]
        public void pack_request_to_bytes_then_unpack_it()
        {
            var piratepackage = new ProtocolPackage();

            var requestInfo = new PirateXRequestInfo(new NameValueCollection()
            {
                {"c","test" },
                { "t","123456"}
            }, new NameValueCollection()
            {
                 {"p1","1" },
                 {"p2","2" },
            });

            var requestPackage = new PirateXRequestPackage()
            {
                HeaderBytes = Encoding.UTF8.GetBytes($"{String.Join("&", requestInfo.Headers.AllKeys.Select(a => a + "=" + requestInfo.Headers[a]))}"),
                ContentBytes = Encoding.UTF8.GetBytes($"{String.Join("&", requestInfo.QueryString.AllKeys.Select(a => a + "=" + requestInfo.QueryString[a]))}")
            };

            var unpackrequestpack = piratepackage.PackPacketToBytes(requestPackage);

            var requestInfo2 = new PirateXRequestInfo(piratepackage.UnPackToPacket(unpackrequestpack));

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
            var piratepack = new ProtocolPackage(){};
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

            var bytes = piratepack.PackPacketToBytes(responsepack);

            var unpackresponsepack = piratepack.UnPackToPacket(bytes);

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


        [Test]
        public void pack_request_to_bytes_then_unpack_it_with_crypto()
        {
            var clientPackage = new ProtocolPackage()
            {
                CryptoByte = new BitArray(new bool[8]
                {
                    false, false, false, false,
                    false, false, false, true
                }).ConvertToByte(),
            };

            var serverPackage = new ProtocolPackage()
            {
                CryptoByte = new BitArray(new bool[8]
                {
                    false, false, false, false,
                    false, false, false, true
                }).ConvertToByte(),
            };

            var clientKeys = KeyGenerator.MakeKey(100);
            var serverKeys = KeyGenerator.MakeKey(200);
            //client
            clientPackage.PackKeys = clientKeys;
            clientPackage.UnPackKeys = serverKeys;
            //server
            serverPackage.PackKeys = serverKeys;
            serverPackage.UnPackKeys = clientKeys;

            var requestInfo = new PirateXRequestInfo(new NameValueCollection()
            {
                {"c","test" },
                { "t","123456"}
            }, new NameValueCollection()
            {
                {"p1","1" },
                {"p2","2" },
            });

            var requestPackage = new PirateXRequestPackage()
            {
                HeaderBytes = Encoding.UTF8.GetBytes($"{String.Join("&", requestInfo.Headers.AllKeys.Select(a => a + "=" + requestInfo.Headers[a]))}"),
                ContentBytes = Encoding.UTF8.GetBytes($"{String.Join("&", requestInfo.QueryString.AllKeys.Select(a => a + "=" + requestInfo.QueryString[a]))}")
            };

            var unpackrequestpack = clientPackage.PackPacketToBytes(requestPackage);

            Console.WriteLine($"[{string.Join(",", unpackrequestpack)}]");

            var requestInfo2 = new PirateXRequestInfo(serverPackage.UnPackToPacket(unpackrequestpack));

            Assert.IsNotEmpty(requestInfo.C);
            Assert.IsNotEmpty(requestInfo.Key);

            Assert.IsNotEmpty(requestInfo2.C);


            Assert.AreEqual($"{String.Join("&", requestInfo.Headers.AllKeys.Select(a => a + "=" + requestInfo.Headers[a]))}"
                , $"{String.Join("&", requestInfo2.Headers.AllKeys.Select(a => a + "=" + requestInfo2.Headers[a]))}");

            Assert.AreEqual($"{String.Join("&", requestInfo.QueryString.AllKeys.Select(a => a + "=" + requestInfo.QueryString[a]))}"
                , $"{String.Join("&", requestInfo2.QueryString.AllKeys.Select(a => a + "=" + requestInfo2.QueryString[a]))}");

        }

        [Test]
        public void client_test()
        {
            var clientPackage = new ProtocolPackage()
            {
                CryptoByte = new BitArray(new bool[8]
                {
                    false, false, false, false,
                    false, false, false, true
                }).ConvertToByte(),
                ZipEnable = false,
            };

            var serverPackage = new ProtocolPackage()
            {
                CryptoByte = new BitArray(new bool[8]
                {
                    false, false, false, false,
                    false, false, false, true
                }).ConvertToByte(),
            };

            var clientKeys = KeyGenerator.MakeKey(1);
            clientPackage.PackKeys = clientKeys;

            var requestPackage = new PirateXRequestPackage()
            {
                HeaderBytes = Encoding.UTF8.GetBytes($"hello"),
                ContentBytes = Encoding.UTF8.GetBytes("hello")
            };

            var unpackrequestpack = clientPackage.PackPacketToBytes(requestPackage);

            Console.WriteLine(string.Join(",", unpackrequestpack));
        }

        [Test]
        public void key()
        {
            var keys = KeyGenerator.MakeKey(1);

            Console.WriteLine(string.Join(",",keys));


            Console.WriteLine(string.Join(",",Encoding.UTF8.GetBytes("")));
        }

        [Test]
        public void crypto()
        {

            var keys = KeyGenerator.MakeKey(1);
            var xxtea = new PirateX.Protocol.XXTea();

            var packet = xxtea.Encode(Encoding.UTF8.GetBytes("hello"), keys);
            Console.WriteLine(string.Join(",", packet));
        }
    }
}
