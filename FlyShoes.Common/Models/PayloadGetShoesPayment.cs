using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyShoes.Common.Models
{
    public class PayloadGetShoesPayment
    {
        public List<int> ShoesIDs { get; set; }

        public int UserID { get; set; }
    }
}
