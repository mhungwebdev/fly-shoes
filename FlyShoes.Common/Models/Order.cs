using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyShoes.Common.Models
{
    public class Order : BaseModel
    {
        public int? OrderID { get; set; }
        public int? UserID { get; set; }
        public int? VoucherID { get; set; }
        public int? Status { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverPhone { get; set; }
        public string ReceiverAddress { get; set; }
    }
}
