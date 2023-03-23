using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyShoes.Common.Models
{
    public class FlyEmail
    {
        public string From { get; set; }

        public string To { get; set; }

        public List<string> Cc { get; set; }

        public string Subject { get; set; }

        public string EmailContent { get; set; }
    }
}
