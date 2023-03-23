using FlyShoes.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyShoes.Common.Models
{
    public class RankCustomer : BaseModel
    {
        public int? RankID { get; set; }
        public string RankName { get; set; }
        public int Min { get; set; }
        public int Max { get; set; }
        public LevelCustomerEnum Level { get; set; }
    }
}
