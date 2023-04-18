using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyShoes.Common.Models
{
    [ConfigTable(tableName:"OrderDetail")]
    public class OrderDetail : BaseModel
    {
        [PrimaryKey]
        public int? OrderDetailID { get; set; }
        [Required]
        public int? ShoesID { get; set; }
        
        [Required]
        public int? ShoesDetailID { get; set; }

        [Required]
        public int? OrderID { get; set; }

        [Required]
        public int? Quantity { get; set; }

        public int? VoucherID { get; set; }

        public decimal? TotalMoney { get; set; }
    }
}
