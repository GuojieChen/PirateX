using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Xml.Serialization;

namespace PirateX.ApiHelper.Controllers
{
    public class CommentsController : ApiController
    {
        public CommentsDoc Get()
        {
            var file = $"{AppDomain.CurrentDomain.BaseDirectory}App_Data\\PokemonIII.Domain.xml";

            XmlSerializer deserializer = new XmlSerializer(typeof(CommentsDoc));
            TextReader reader = new StreamReader(file);
            object obj = deserializer.Deserialize(reader);
            CommentsDoc commentsDoc = (CommentsDoc)obj;
            reader.Close();

            return commentsDoc;
        }
    }
}
