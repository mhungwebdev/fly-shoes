using FlyShoes.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyShoes.Common.Models
{
    [ConfigTable(tableName:"Color")]
    public class Color : BaseModel
    {
        [PrimaryKey]
        public int? ColorID { get; set; }

        [Unique,Required,Length(maxLength:100)]
        public string ColorName { get; set; }

        [Unique,Required]
        public string ColorCode { get; set; }
    }
}
