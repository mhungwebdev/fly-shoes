using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyShoes.Common.Models
{
    public class Category : BaseModel
    {
        public int? CategoryID { get; set; }
        public int? CategoryName { get; set; }
        public int? CategoryDescription { get; set; }
        public int? VoucherID { get; set; }
    }
}
