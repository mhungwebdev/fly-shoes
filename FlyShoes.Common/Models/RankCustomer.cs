using FlyShoes.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyShoes.Common.Models
{
    [ConfigTable(tableName:"RankCustomer")]
    public class RankCustomer : BaseModel
    {
        [PrimaryKey]
        public int? RankID { get; set; }
        [Required,Unique]
        public string RankName { get; set; }

        [Required]
        public int Min { get; set; }
        
        [Required]
        public int Max { get; set; }

        [Required]
        public LevelCustomerEnum Level { get; set; }
    }
}
