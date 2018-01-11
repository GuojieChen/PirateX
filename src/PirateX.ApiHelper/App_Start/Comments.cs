using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace PirateX.ApiHelper
{
    [XmlRoot("doc")]
    public class CommentsDoc
    {
        [XmlElement("assembly")]
        public CommentsAssembly Assembly { get; set; }

        [XmlElement("members")]
        public CommentsMembers Members { get; set; }
    }


    public class CommentsAssembly
    {
        [XmlElement("name")]
        public string Name { get; set; }
    }

    public class CommentsMember
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlElement("summary")]
        public XmlNode Summary { get; set; }
    }

    public class CommentsMembers
    {
        [XmlElement("member")]
        public List<CommentsMember> Members { get; set; }
    }


}