using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace FlyShoes.Common.Models
{
    [ConfigTable("PaymentInfo")]
    public class PaymentInfo : BaseModel
    {
        [PrimaryKey]
        public int PaymentInfoID { get; set; }

        public int OrderID { get; set; }
        public string BankCode { get; set; }
        public string Amount { get; set; }
        public string TransactionNo { get; set; }
    }
}
