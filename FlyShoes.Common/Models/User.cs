using FlyShoes.Common.Attributes;
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
        public int? UserID { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public string FirebaseId { get; set; }

        public string Address { get; set; }

        public bool IsAdmin { get; set; }

        public int? RankID { get; set; }

        public int? AmountSpent { get; set; }

        public string Phone { get; set; }
    }
}
