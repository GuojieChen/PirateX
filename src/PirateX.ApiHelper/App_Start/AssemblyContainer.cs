using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using Newtonsoft.Json;
using PirateX.Core.Actor;
using PirateX.Core.Utils;
using TestDataGenerator;

namespace PirateX.ApiHelper.App_Start
{
    public class AssemblyContainer
    {
        private IDictionary<string,ApiGroup> _dic = new Dictionary<string, ApiGroup>();
        private List<ApiGroup> _list = new List<ApiGroup>();

        public static AssemblyContainer Instance = new AssemblyContainer();

        private AssemblyContainer()
        {

        }

        public void Load(params Assembly[] list)
        {
            foreach (var assembly in list)
            {
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
            foreach (var type in assembly.GetTypes())
            {
                if (!type.IsClass)
                    continue;

                if (typeof(IAction).IsAssignableFrom(type))
                //if((type as IAction)!=null)
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

                var responseDeses = gtype.GetProperties().Select(item => 
                    new ResponseDes()
                    {
                        Name = item.Name,
                        Type = item.PropertyType,
                        PpDoc = item.GetCustomAttribute<ApiDocAttribute>()
                    });

                detail.ResponseDeses = responseDeses;
                detail.Proto = gtype.GetProto();
                //detail.Json = JsonConvert.SerializeObject(new Catalog().CreateInstance(gtype));
            }

            return detail;
        }
    }
}