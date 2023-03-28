using FlyShoes.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FlyShoes.Common.Models
{
    [ConfigTable(tableName: "User")]
    public class User : BaseModel
    {
        [PrimaryKey]
        public int? UserID { get; set; }

        [Required,Length(maxLength:100)]
        public string FullName { get; set; }

        [Required,Email]
        public string Email { get; set; }

        [Required,Unique]
        public string FirebaseID { get; set; }

        public string Address { get; set; }

        [NotMap]
        public bool IsAdmin { get; set; }

        public int? RankID { get; set; }

        public int? AmountSpent { get; set; }

        [Phone]
        public string Phone { get; set; }
    }
}
