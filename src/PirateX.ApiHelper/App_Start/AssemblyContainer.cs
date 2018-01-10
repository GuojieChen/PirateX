using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using Newtonsoft.Json;
using PirateX.Core.Actor;
using PirateX.Core.Domain.Entity;
using PirateX.Core.Utils;
using ProtoBuf;
using TestDataGenerator;

namespace PirateX.ApiHelper.App_Start
{
    public class AssemblyContainer
    {
        private IDictionary<string,ApiGroup> _dic = new Dictionary<string, ApiGroup>();
        private List<ApiGroup> _list = new List<ApiGroup>();
        private IDictionary<string, Assembly> _assemblies = new Dictionary<string, Assembly>();

        public static AssemblyContainer Instance = new AssemblyContainer();

        private AssemblyContainer()
        {

        }

        public void Load(params Assembly[] list)
        {
            foreach (var assembly in list)
            {
                if(Equals(assembly.GetName().Name,"PirateX.Core"))
                    continue;

                var api = GetApiGroup(assembly);

                if (NeedLoad(assembly))
                {
                    _dic.Add(api.ModelId, api);
                    _list.Add(api);
                }
            }
        }

        private bool NeedLoad(Assembly assembly)
        {
            _assemblies.Add(assembly.ManifestModule.ModuleVersionId.ToString("N"), assembly);
            foreach (var type in assembly.GetTypes())
            {
                if (!type.IsClass)
                    continue;

                if (typeof(IAction).IsAssignableFrom(type))
                    return true;
            }

            return false; 
        }

        private ApiGroup GetApiGroup(Assembly assembly)
        {
            var group = new ApiGroup() { Assembly = assembly };

            foreach (var type in assembly.GetTypes())
            {
                if (!type.IsClass)
                    continue;
                
                if (typeof(IAction).IsAssignableFrom(type))
                    group.Types.Add(type);
            }

            return group;
        }

        public ApiGroup GetApiGroup(string modelid)
        {
            if (!string.IsNullOrEmpty(modelid) && _dic.ContainsKey(modelid))
                return _dic[modelid];

            return _list[0];
        }

        public Type GetRequestType(string modelid, string typeguid)
        {
            if (string.IsNullOrEmpty(modelid) || string.IsNullOrEmpty(typeguid))
                return null;

            if (!_dic.ContainsKey(modelid))
                return null;

            return _dic[modelid].Types.FirstOrDefault(item => item.GUID.ToString("N") == typeguid);
        }

        public List<ApiGroup> GetGroups()
        {
            return _list;
        }

        public TypeDetails GetTypeDetails(Type type)
        {
            if (type == null)
                return null;

            var detail = new TypeDetails();
            detail.Name = type.Name;
            detail.ApiDoc = type.GetCustomAttribute<ApiDocAttribute>();
            detail.RequestDocs = type.GetCustomAttributes<RequestDocAttribute>();
            if (type.BaseType.GenericTypeArguments.Any())
            {
                var gtype = type.BaseType.GenericTypeArguments[0];
                detail.ResponseDeses = GetResponseDeses(gtype);
                detail.Proto = gtype.GetProto();
                //detail.Json = JsonConvert.SerializeObject(new Catalog().CreateInstance(gtype));
            }

            return detail;
        }

        private IEnumerable<ResponseDes> GetResponseDeses(Type type)
        {
            if (type != null)
            {
                var assembly = type.Assembly;

                return type.GetProperties().Where(item=>item.GetCustomAttribute<ProtoMemberAttribute>()!=null).Select(item =>
                {
                    var des = new ResponseDes()
                    {
                        Name = item.Name,
                        //PpDoc = item.GetCustomAttribute<ApiDocAttribute>(),
                        Commonts = CommentsDocContainer.Instance.GetCommontsMember(CommentsDocContainer.Instance.GetCommentsDoc(assembly),type,item),
                        IsPrimitive = item.PropertyType.IsPrimitive,
                    };

                    if (item.PropertyType.IsPrimitive)
                    {
                        des.ModelId = item.PropertyType.Assembly.ManifestModule.ModuleVersionId.ToString("N");
                        des.TypeName = item.PropertyType.Name;
                        des.TypeId = item.PropertyType.GUID.ToString("N");
                    }
                    else if (item.PropertyType.IsGenericType)
                    {
                        des.ModelId = item.PropertyType.GenericTypeArguments[0].Assembly.ManifestModule.ModuleVersionId.ToString("N");
                        des.TypeName = item.PropertyType.GenericTypeArguments[0].Name;
                        des.TypeId = item.PropertyType.GenericTypeArguments[0].GUID.ToString("N");
                    }
                    else
                    {
                        des.ModelId = item.PropertyType.Assembly.ManifestModule.ModuleVersionId.ToString("N");
                        des.TypeName = item.PropertyType.Name;
                        des.TypeId = item.PropertyType.GUID.ToString("N");
                    }


                    return des;
                });
            }

            return null;
        }

        public IEnumerable<ResponseDes> GetResponseDeses(string modelid,string id)
        {
            var assembly = _assemblies[modelid];
            var type = assembly.GetTypes().FirstOrDefault(item => Equals(id, item.GUID.ToString("N")));

            return GetResponseDeses(type);
        }
    }
}