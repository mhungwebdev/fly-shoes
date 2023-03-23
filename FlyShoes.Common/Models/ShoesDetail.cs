using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyShoes.Common.Models
{
    public class ShoesDetail : BaseModel
    {
        public int? ShoesDetailID { get; set; }
        public int? ShoesID { get; set; }
        public int? SizeID { get; set; }
        public int? ColorID { get; set; }
        public int Quantity { get; set; }
        public string ShoesImage { get; set; }
    }
}
