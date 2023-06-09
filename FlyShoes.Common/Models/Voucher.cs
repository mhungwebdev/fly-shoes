﻿using FlyShoes.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace FlyShoes.Common.Models
{
    [ConfigTable(tableName:"Voucher")]
    public class Voucher : BaseModel
    {
        [PrimaryKey]
        public int? VoucherID { get; set; }
        public string VoucherTitle { get; set; }
        public VoucherFormulaEnum FormulaType { get; set; }
        public int? VoucherValue { get; set; }
        public int? Quantity { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }

        [NotMap]
        public List<int> ShoesIDApply { get; set; }
    }
}
