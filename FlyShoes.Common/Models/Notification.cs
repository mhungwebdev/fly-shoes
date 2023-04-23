using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyShoes.Common.Models
{
    public class Notification
    {
        public int? UserID { get; set; }

        public string Message { get; set; }

        public bool IsRead { get; set; }

        public string Link { get; set; }

        public decimal SortOrder { get; set; }

        public Notification()
        {
        }
    }
}
