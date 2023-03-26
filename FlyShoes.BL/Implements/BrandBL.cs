using FlyShoes.BL.Interfaces;
using FlyShoes.Common.Models;
using FlyShoes.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyShoes.BL.Base
{
    public class BrandBL : BaseBL<Brand>,IBrandBL
    {
        public BrandBL(IDatabaseService databaseService):base(databaseService)
        {

        }
    }
}
