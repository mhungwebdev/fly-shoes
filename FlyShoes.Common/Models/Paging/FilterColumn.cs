using FlyShoes.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyShoes.Common.Models
{
    public class FilterColumn
    {
        /// <summary>
        /// Field filter
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// Filter operator
        /// </summary>
        public FilterOperator FilterOperator { get; set; }

        public object Value { get; set; }

        public FilterColumn() { 
            this.FieldName = string.Empty;
            this.FilterOperator = FilterOperator.NotEqual;
        }
    }
}
