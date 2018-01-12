using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateX.Middleware.LetterSystem
{
    /// <summary>
    /// 活动归档生成信件
    /// </summary>
    public interface IArchiveToLetter
    {
        /// <summary>
        /// 数据归档，例如排行，
        /// </summary>
        void Archive();

        /// <summary>
        /// 生成信件
        /// </summary>
        /// <param name="rid"></param>
        void Builder(int rid);
    }
}
