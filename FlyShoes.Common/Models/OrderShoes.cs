using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyShoes.Common.Models
{
    [ConfigTable(tableName:"OrderShoes")]
    public class OrderShoes : BaseModel
    {
        [PrimaryKey]
        public int? OrderID { get; set; }
        [Required]
        public int? UserID { get; set; }
        public int? VoucherID { get; set; }
        [Required]
        public int? Status { get; set; }
        [Required,Length(maxLength:100)]
        public string ReceiverName { get; set; }
        [Required,Phone]
        public string ReceiverPhone { get; set; }
        [Required]
        public string ReceiverAddress { get; set; }
    }
}
