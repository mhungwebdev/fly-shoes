using FlyShoes.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyShoes.Common.Models
{
    [ConfigTable(tableName: "Brand")]
    public class Brand : BaseModel
    {
        [PrimaryKey]
        public int? BrandID { get; set; }

        [Unique,Required,Length(maxLength:100)]
        public string BrandName { get; set; }

        [Required]
        public string BrandSlogan { get; set; }
        public string DescriptionShot { get; set; }

        [Required]
        public string DescriptionDetail { get; set; }

        [Required]
        public string BrandLogo { get; set; }
        public int? VoucherID { get; set; }
    }
}
