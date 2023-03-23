using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyShoes.Common.Models
{
    public class OrderDetail : BaseModel
    {
        public int? OrderDetailID { get; set; }
        public int? ShoesID { get; set; }
        public int? ShoesDetailID { get; set; }
        public int? OrderID { get; set; }
        public int? Quatity { get; set; }
        public int? VoucherID { get; set; }
    }
}
