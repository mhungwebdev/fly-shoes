using FlyShoes.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyShoes.Common.Models
{
    [ConfigTable(tableName:"OrderShoes",fieldSearch: "ReceiverName;ReceiverPhone;ReceiverAddress")]
    public class OrderShoes : BaseModel
    {
        [PrimaryKey]
        public int? OrderID { get; set; }
        [Required]
        public int? UserID { get; set; }
        [Required]
        public int? Status { get; set; }
        [Required,Length(maxLength:100)]
        public string ReceiverName { get; set; }
        [Required,Phone]
        public string ReceiverPhone { get; set; }
        [Required]
        public string ReceiverAddress { get; set; }
        [NotMap]
        [Detail("SELECT * FROM OrderDetail WHERE OrderID = @MasterID", "OrderDetails",typeof(List<OrderDetail>))]
        public List<OrderDetail> OrderDetails { get; set; }

        public decimal TotalBill { get; set; }

        public PaymentMethod PaymentMethod { get; set; }

        public bool PaymentStatus { get; set; }

        [NotMap]
        public string BankCode { get; set; }
    }
}
