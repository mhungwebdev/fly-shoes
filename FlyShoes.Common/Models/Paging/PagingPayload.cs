using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyShoes.Common.Models
{
    public class PagingPayload
    {
        public int PageSize { get; set; }

        public int PageIndex { get; set; }

        public string Keyword { get; set; }

        public SortOrder SortOrder { get; set; }

        public List<FilterColumn> FilterColumns { get; set; }

        public PagingPayload() {
            this.PageSize = 15;
            this.PageIndex = 1;
            this.Keyword = "";
            this.SortOrder = new SortOrder();
            this.FilterColumns= new List<FilterColumn>();
        }
    }
}
