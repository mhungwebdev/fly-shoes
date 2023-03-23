using FlyShoes.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace FlyShoes.Common.Models
{
    public class Voucher : BaseModel
    {
        public int? VoucherID { get; set; }
        public string VoucherTitle { get; set; }
        public VoucherTypeEnum VoucherType { get; set; }
        public int? TargetApplyID { get; set; }
        public VoucherFormulaEnum FormulaType { get; set; }
        public int? VoucherValue { get; set; }
        public int? Quantity { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string IDApplied { get; set; }
    }
}
