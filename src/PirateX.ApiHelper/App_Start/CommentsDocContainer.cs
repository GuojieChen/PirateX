using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Xml.Serialization;

namespace PirateX.ApiHelper.App_Start
{
    public class CommentsDocContainer
    {
        private IDictionary<string, CommentsDoc> _dic = new Dictionary<string, CommentsDoc>();

        private static CommentsDocContainer _instance = null;
        private static object _lockHelper = new object();
        public static CommentsDocContainer Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lockHelper)
                    {
                        if(_instance==null)
                        {
                            _instance = new CommentsDocContainer();
                            _instance.Load(Directory.GetFiles($"{AppDomain.CurrentDomain.BaseDirectory}App_Data").Where(item => item.EndsWith(".xml")).ToArray());
                        }
                    }
                }

                return _instance;
            }
        }
        public static void SetInstanceNull()
        {
            _instance = null;
        }

        private CommentsDocContainer()
        {

        }

        public void Load(params string[] files)
        {
            foreach (var file in files)
            {
                var filename = System.IO.Path.GetFileName(file);

                if (!AssemblyContainer.Instance.GetAssemblyXmlList().Contains(filename))
                    continue;

                var deserializer = new XmlSerializer(typeof(CommentsDoc));
                using (var reader = new StreamReader(file))
                {
                    object obj = deserializer.Deserialize(reader);
                    var commentsDoc = (CommentsDoc)obj;

                    _dic.Add(commentsDoc.Assembly.Name, commentsDoc);
                }
            }
        }

        public CommentsDoc GetCommentsDoc(Assembly assembly)
        {
            var assemblyName = assembly.FullName.Substring(0, assembly.FullName.IndexOf(','));

            if (!_dic.ContainsKey(assemblyName))
                return null;

            return _dic[assemblyName];
        }

        public CommentsMember GetPropertyCommontsMember(CommentsDoc doc, Type type, PropertyInfo propertyInfo)
        {
            var member = doc.Members.Members.FirstOrDefault(item => Equals(item.Name, $"P:{type.FullName}.{propertyInfo.Name}"));

            return member;
        }

        public CommentsMember GetTypeCommontsMember(CommentsDoc doc, Type type)
        {

            var member = doc.Members.Members.FirstOrDefault(item => Equals(item.Name, $"T:{type.FullName}"));

            return member;
        }
    }
}