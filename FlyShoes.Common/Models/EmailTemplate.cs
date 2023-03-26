using FlyShoes.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyShoes.Common.Models
{
    [ConfigTable(tableName:"EmailTemplate")]
    public class EmailTemplate : BaseModel
    {
        [PrimaryKey]
        public int? EmailTemplateID { get; set; }
        
        [Required]
        public EmailTypeEnum EmailType { get; set; }

        [Required]
        public string EmailContent { get; set; }
    }
}
