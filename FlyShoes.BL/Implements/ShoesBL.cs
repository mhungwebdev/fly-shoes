﻿using FlyShoes.BL.Interfaces;
using FlyShoes.Common.Models;
using FlyShoes.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyShoes.BL.Implements
{
    public class ShoesBL : BaseBL<Shoes>,IShoesBL
    {
        public ShoesBL(IDatabaseService databaseService):base(databaseService)
        {

        }

        public async Task<decimal> GetMaxPrice()
        {
            var command = "SELECT MAX(Price) FROM Shoes";
            return await _dataBaseService.ExecuteScalarUsingCommandTextAsync<decimal>(command);
        }
    }
}
