using System;
using System.Collections.Generic;
using System.IO;

namespace PirateX.Core
{
    //http://www.codeproject.com/Articles/16068/Zeta-Resource-Editor
    public static class I18N
    {

        private static string _defaultLocal = "zh-CN";

        public static string _defaultPath { get; set; }

        private static readonly IDictionary<string, IDictionary<string, string>> _source = new Dictionary<string, IDictionary<string, string>>();

        /// <summary> 初始化语言配置
        /// </summary>
        public static void Init()
        {
            if (string.IsNullOrEmpty(_defaultPath))
                _defaultPath = $"{AppDomain.CurrentDomain.BaseDirectory}{Path.DirectorySeparatorChar}i18n{Path.DirectorySeparatorChar}i18n.csv";

            if (!File.Exists(_defaultPath))
                throw new FileNotFoundException($"File not found :{_defaultPath}");

            var isTitle = true;

            var locallist = new List<string>();
            using (var reader = new CsvFileReader(_defaultPath))
            {
                var row = new CsvRow();
                while (reader.ReadRow(row))
                {
                    if (isTitle)
                    {//标题
                        var isKey = true;
                        foreach (string s in row)
                        {
                            if (isKey)
                            {//KEY
                                isKey = false; 
                                continue;
                            }

                            _source.Add(s, new Dictionary<string, string>());
                            locallist.Add(s);
                        }

                        isTitle = false;
                    }
                    else
                    {
                        var key = "";

                        for (var i = 0; i < row.Count && i <= locallist.Count; i++)
                        {
                            if (i == 0)
                            {
                                key = row[i];
                                continue;
                            }

                            _source[locallist[i-1]].Add(key, row[i]);
                        }
                    }
                }
            }
        }

        public static string GetLanguageValue(this string key)
        {
            return GetLanguageValue(key, _defaultLocal);
        }

        public static string GetLanguageValue(this string key,string local)
        {
            if (!_source.ContainsKey(local))
                local = _defaultLocal;

            if (!_source[local].ContainsKey(key))
                return string.Empty;

            return _source[local][key];
        }
    }
}
