using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyShoes.Common.Models
{
    public class Shoes : BaseModel
    {
        public int? ShoesID { get; set; }
        public string ShoesName { get; set; }
        public decimal Price { get; set; }
        public string ShoesImage { get; set; }
        public int? CategoryID { get; set; }
        public int? BrandID { get; set; }
    }
}
