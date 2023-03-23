using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyShoes.Common.Models
{
    public class Brand : BaseModel
    {
        public int? BrandID { get; set; }
        public string BrandName { get; set; }
        public string BrandSologan { get; set; }
        public string DescriptionShot { get; set; }
        public string DescriptionDetail { get; set; }
        public string BrandLogo { get; set; }
        public int? VoucherID { get; set; }
    }
}
