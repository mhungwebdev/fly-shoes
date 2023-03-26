﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyShoes.Common.Models
{
    [ConfigTable(tableName:"Shoes",fieldSearch:"ShoesName",isMaster:true)]
    public class Shoes : BaseModel
    {
        [PrimaryKey]
        public int? ShoesID { get; set; }

        [Unique,Required]
        public string ShoesName { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public string ShoesImage { get; set; }
        
        [Required]        
        public int? CategoryID { get; set; }

        [Required]
        public string CategoryName { get; set; }

        [Required]
        public int? BrandID { get; set; }

        [Required]
        public string BrandName { get; set;}
    }
}
