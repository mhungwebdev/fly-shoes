using FlyShoes.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyShoes.Common.Constants
{
    /// <summary>
    /// Constant cho sort
    /// </summary>
    public class SortConstant
    {
        /// <summary>
        /// Tăng dần
        /// </summary>
        public static readonly string ASC = "ASC";

        /// <summary>
        /// Giảm dần
        /// </summary>
        public static readonly string DESC = "DESC";

        /// <summary>
        /// Lấy sort text theo sorttype
        /// </summary>
        /// <param name="sortType"></param>
        /// <returns></returns>
        static string GetSortText(SortType sortType)
        {
            var result = ASC;
            switch (sortType)
            {
                case SortType.ASC:
                    result = ASC;
                    break;
                case SortType.DESC:
                    result = DESC;
                    break;
            }
            return result;
        }
    }
}
