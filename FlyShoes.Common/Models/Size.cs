using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyShoes.Common.Models
{
    public class Size : BaseModel
    {
        public int? SizeID { get; set; }
        public string SizeName { get; set; }
        public string Description { get; set; }
        public string Note { get; set; }
    }
}
