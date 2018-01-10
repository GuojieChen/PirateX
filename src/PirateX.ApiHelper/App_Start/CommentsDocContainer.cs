using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Xml.Serialization;

namespace PirateX.ApiHelper.App_Start
{
    public class CommentsDocContainer
    {
        private IDictionary<string,CommentsDoc> _dic = new Dictionary<string, CommentsDoc>();

        public static CommentsDocContainer Instance = new CommentsDocContainer();
        private CommentsDocContainer()
        {

        }

        public void Load(params  string[] files)
        {
            foreach (var file in files)
            {
                var deserializer = new XmlSerializer(typeof(CommentsDoc));
                using (var reader = new StreamReader(file))
                {
                    object obj = deserializer.Deserialize(reader);
                    var commentsDoc = (CommentsDoc)obj;

                    _dic.Add(commentsDoc.Assembly.Name,commentsDoc);
                }
            }
        }

        public CommentsDoc GetCommentsDoc(Assembly assembly)
        {
            var assemblyName = assembly.FullName.Substring(0,assembly.FullName.IndexOf(','));

            if (!_dic.ContainsKey(assemblyName))
                return null;

            return _dic[assemblyName];
        }

        public CommentsMember GetCommontsMember(CommentsDoc doc,Type type, PropertyInfo propertyInfo)
        {
            var member = doc.Members.Members.FirstOrDefault(item => Equals(item.Name, $"P:{type.FullName}.{propertyInfo.Name}"));

            return member;
        }

    }
}