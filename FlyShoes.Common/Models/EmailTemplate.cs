using FlyShoes.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyShoes.Common.Models
{
    public class EmailTemplate : BaseModel
    {
        public int? EmailTemplateID { get; set; }
        public EmailTypeEnum EmailType { get; set; }
        public string EmailContent { get; set; }
    }
}
