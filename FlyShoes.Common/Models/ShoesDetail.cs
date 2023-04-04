using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyShoes.Common.Models
{
    [ConfigTable(tableName:"ShoesDetail")]
    public class ShoesDetail : BaseModel
    {
        [PrimaryKey]
        public int? ShoesDetailID { get; set; }

        [Required]
        public int? ShoesID { get; set; }
        
        [Required]
        public int? SizeID { get; set; }

        [Required]
        public string SizeName { get; set; }

        [Required]
        public int? ColorID { get; set; }
        
        [Required]
        public string ColorName { get; set; }

        [Required]
        public string ColorCode { get; set; }
        
        [Required]
        public int Quantity { get; set; }
        
        [Required]
        public string ShoesImage { get; set; }
    }
}
