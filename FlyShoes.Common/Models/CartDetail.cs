using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyShoes.Common.Models
{
    [ConfigTable(tableName:"CartDetail")]
    public class CartDetail : BaseModel
    {
        [PrimaryKey]
        public int? CartDetailID { get; set; }

        [Required]
        public int? UserID { get; set; }

        [Required]
        public int? ShoesID { get; set; }

        [NotMap]
        public string ShoesImages { get; set; }

        [NotMap]
        public string ShoesName { get; set; }

        [NotMap]
        public int? VoucherID { get; set; }

        [NotMap]
        public int? Total { get; set; }

        [NotMap]
        public Voucher? Voucher { get; set; }

        [NotMap]
        public decimal Price { get; set; }
    }
}
