using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateX.Core.Domain.Entity
{
    public interface IEntityDistrict
    {
        ///// <summary>
        ///// 初始的服ID
        ///// </summary>
        //int PrimeDid { get; set; }

        /// <summary>
        /// 当前服ID
        /// </summary>
        int Did { get; set; }
    }
}
