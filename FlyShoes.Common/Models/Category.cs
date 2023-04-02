using FlyShoes.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyShoes.Common.Models
{
    [ConfigTable(tableName:"Category",relatedTables:"Shoes")]
    public class Category : BaseModel
    {
        [PrimaryKey]
        public int? CategoryID { get; set; }

        [Required,Unique,Length(maxLength:100)]
        public string CategoryName { get; set; }

        [Required]
        public string? CategoryDescription { get; set; }
        public int? VoucherID { get; set; }
    }
}
