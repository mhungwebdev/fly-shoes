using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyShoes.Common.Models
{
    public class CartDetail : BaseModel
    {
        public int? CartDetailID { get; set; }
        public int? UserID { get; set; }
        public int? ShoesDetailID { get; set; }
        public int? Quantity { get; set; }
        public string ColorName { get; set; }
    }
}
