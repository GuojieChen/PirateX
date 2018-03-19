using PirateX.GMSDK.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateX.GMSDK.Demo
{
    public class GuildLetter
    {
        public string Title { get; set; }

        public string Content { get; set; }
    }

    public class GMUIGuildLetterMap: GMUIItemMap<GuildLetter>
    {
        public GMUIGuildLetterMap()
        {
            Map<GMUITextBoxPropertyMap>(item => item.Title)
                .ToDisplayName("标题");

            Map<GMUITextBoxPropertyMap>(item => item.Content)
                .ToDisplayName("内容");
        }
    }
}
