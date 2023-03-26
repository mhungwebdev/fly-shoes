using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyShoes.Common.Models
{
    [ConfigTable(tableName:"CartDetail")]
    public class CartDetail : BaseModel
    {
        [PrimaryKey]
        public int? CartDetailID { get; set; }

        [Required]
        public int? UserID { get; set; }
        
        [Required]
        public int? ShoesDetailID { get; set; }
       
        [Required]
        public int? Quantity { get; set; }
        
        [Required]
        public string ColorName { get; set; }
    }
}
