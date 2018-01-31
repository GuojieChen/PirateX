using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using Newtonsoft.Json;
using PirateX.Core;
using ProtoBuf;
using TestDataGenerator;

namespace PirateX.ApiHelper.App_Start
{
    public class AssemblyContainer
    {
        private IDictionary<string, ApiGroup> _dic = null;
        private List<ApiGroup> _list = null;
        private List<ApiGroup> _apiGroupList = null; 
        private IDictionary<string, Assembly> _assemblies = null;
        private List<string> _assemblyXmlList = null;
        private static AssemblyContainer _instance = null;

        private static object _lockHelper = new object();

        public static AssemblyContainer Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lockHelper)
                    {
                        if (_instance == null)
                        {
                            _instance = new AssemblyContainer();
                            _instance.Load();
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

        private void Load()
        {
            _dic = new Dictionary<string, ApiGroup>();
            _list = new List<ApiGroup>();
            _assemblies = new Dictionary<string, Assembly>();
            _assemblyXmlList = new List<string>();
            _apiGroupList = new List<ApiGroup>();


            var list = new List<Assembly>();
            foreach (var file in Directory.GetFiles($"{AppDomain.CurrentDomain.BaseDirectory}App_Data").Where(item => item.EndsWith(".dll")))
            {
                var systemExists = Directory.GetFiles($"{AppDomain.CurrentDomain.BaseDirectory}bin").Select(Path.GetFileName).ToArray();
                var filename = Path.GetFileName(file);
                if (systemExists.Contains(filename))
                    continue;

                list.Add(Assembly.LoadFrom(file));
            }
            list.Add(typeof(IDistrictContainer).Assembly);
            _instance.Load(list.ToArray());
        }
        private AssemblyContainer()
        {

        }

        public void Load(params Assembly[] list)
        {
            foreach (var assembly in list)
            {
                var api = GetApiGroup(assembly);

                if (api == null)
                    continue;

                if (NeedLoad(api))
                {
                    _dic.Add(api.ModelId, api);
                    _list.Add(api);
                }
            }
        }

        private bool NeedLoad(ApiGroup api)
        {
            foreach (var type in api.Types)
            {
                if (!type.IsClass)
                    continue;

                if (typeof(IAction).IsAssignableFrom(type) || typeof(IEntity).IsAssignableFrom(type))
                {
                    if(!_assemblies.ContainsKey(api.ModelId))
                        _assemblies.Add(api.ModelId, api.Assembly);

                    _assemblyXmlList.Add(api.Assembly.ManifestModule.Name.Replace(".dll", ".xml"));

                    if (typeof(IAction).IsAssignableFrom(type))
                    {
                        _apiGroupList.Add(api);
                        return true;
                    }
                }
            }

            return false;
        }

        private ApiGroup GetApiGroup(Assembly assembly)
        {
            try
            {
                var group = new ApiGroup() { Assembly = assembly };

                foreach (var type in assembly.GetTypes())
                {
                    if (!type.IsClass)
                        continue;

                    if ((typeof(IAction).IsAssignableFrom(type) || typeof(IEntity).IsAssignableFrom(type)) && !type.IsAbstract && !type.IsInterface)
                        group.Types.Add(type);
                }

                return group;
            }
            catch (Exception e)
            {
                return null;
            }
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
            return _apiGroupList;
        }

        public TypeDetails GetTypeDetails(Type type)
        {
            if (type == null)
                return null;

            var detail = new TypeDetails();
            detail.Name = ((IAction)Activator.CreateInstance(type)).Name ?? type.Name;
            detail.Comments = CommentsDocContainer.Instance.GetTypeCommontsMember(CommentsDocContainer.Instance.GetCommentsDoc(type.Assembly), type);
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

                return type.GetProperties().Where(item => item.GetCustomAttribute<ProtoMemberAttribute>() != null).Select(item =>
                    {
                        var des = new ResponseDes()
                        {
                            Name = item.Name,
                        //PpDoc = item.GetCustomAttribute<ApiDocAttribute>(),
                        Commonts = CommentsDocContainer.Instance.GetPropertyCommontsMember(CommentsDocContainer.Instance.GetCommentsDoc(assembly), type, item),
                            IsPrimitive = item.PropertyType.IsPrimitive,
                        };

                        des.ProtoMember = item.GetCustomAttribute<ProtoMemberAttribute>()?.Tag;

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

                            if (typeof(IEnumerable).IsAssignableFrom(item.PropertyType))
                                des.TypeName += "[]";
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

        public IEnumerable<ResponseDes> GetResponseDeses(string modelid, string id)
        {
            var assembly = _assemblies[modelid];
            var type = assembly.GetTypes().FirstOrDefault(item => Equals(id, item.GUID.ToString("N")));

            return GetResponseDeses(type);
        }

        public List<string> GetAssemblyXmlList()
        {
            return _assemblyXmlList;
        }
    }
}