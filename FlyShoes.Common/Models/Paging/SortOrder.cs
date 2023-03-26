using FlyShoes.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyShoes.Common.Models
{
    public class SortOrder
    {
        /// <summary>
        /// Field sort
        /// </summary>
        public string FieldSort { get; set; }

        /// <summary>
        /// Sort type
        /// </summary>
        public SortType SortType { get; set; }

        public SortOrder() { 
            this.FieldSort = string.Empty;
            this.SortType = SortType.DESC;
        }
    }
}
