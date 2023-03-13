using FlyShoes.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyShoes.Common.Constants
{
    /// <summary>
    /// Constant cho công thức filter
    /// </summary>
    public class OperatorConstant
    {
        /// <summary>
        /// Bằng
        /// </summary>
        public static readonly string EQUAL = " = ";
        
        /// <summary>
        /// Khác
        /// </summary>
        public static readonly string NOT_EQUAL = " <> ";
        
        /// <summary>
        /// Lớn hơn
        /// </summary>
        public static readonly string GREATER = " > ";
        
        /// <summary>
        /// Nhỏ hơn
        /// </summary>
        public static readonly string LESS = " < ";

        /// <summary>
        /// Lớn hơn hoặc bằng
        /// </summary>
        public static readonly string GREATER_OR_EQUAL = " >= ";

        /// <summary>
        /// Nhỏ hơn hoặc bằng
        /// </summary>
        public static readonly string LESS_OR_EQUAL = " <= ";

        /// <summary>
        /// Lấy operator text theo filter operator
        /// </summary>
        /// <param name="filterOperator"></param>
        /// <returns></returns>
        static string GetOperatorText(FilterOperator filterOperator)
        {
            var result = EQUAL;
            switch (filterOperator)
            {
                case FilterOperator.Equal:
                    result= EQUAL;
                    break;
                case FilterOperator.NotEqual:
                    result = NOT_EQUAL;
                    break;
                case FilterOperator.Greater:
                    result = GREATER;
                    break;
                case FilterOperator.Less:
                    result = LESS;
                    break;
                case FilterOperator.GreaterOrEqual:
                    result = GREATER_OR_EQUAL;
                    break;
                case FilterOperator.LessOrEqual:
                    result = LESS_OR_EQUAL;
                    break;
            }

            return result;
        }
    }
}
