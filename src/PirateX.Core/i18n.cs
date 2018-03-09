using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateX.Core
{
    public class i18n
    {
        /// <summary>
        /// 语言
        /// </summary>
        public string Language { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }
    }

    public class i18nLetter
    {
        /// <summary>
        /// 语言
        /// </summary>
        public string Language { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }
    }
}
