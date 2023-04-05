using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyShoes.Common.Models
{
    [ConfigTable(tableName:"Size")]
    public class Size : BaseModel
    {
        [PrimaryKey]
        public int? SizeID { get; set; }

        [Required,Unique]
        public string SizeName { get; set; }
    }
}
