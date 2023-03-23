using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyShoes.Common.Models
{
    public class Color : BaseModel
    {
        public int? ColorID { get; set; }
        public string ColorName { get; set; }
        public string ColorCode { get; set; }
        public string Description { get; set; }
    }
}
