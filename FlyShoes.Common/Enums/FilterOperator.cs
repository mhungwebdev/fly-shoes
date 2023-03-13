using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyShoes.Common.Enums
{
    public enum FilterOperator
    {
        /// <summary>
        /// So sánh bằng
        /// </summary>
        Equal = 1,
        /// <summary>
        /// So sánh khác
        /// </summary>
        NotEqual,
        /// <summary>
        /// So sánh lớn hơn
        /// </summary>
        Greater,
        /// <summary>
        /// So sánh nhỏ hơn
        /// </summary>
        Less,
        /// <summary>
        /// So sánh lớn hơn hoặc bằng
        /// </summary>
        GreaterOrEqual,
        /// <summary>
        /// So sánh nhỏ hơn hoặc bằng
        /// </summary>
        LessOrEqual
    }
}
