using FlyShoes.BL.Interfaces;
using FlyShoes.Common.Models;
using FlyShoes.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyShoes.BL.Implements
{
    public class PaymentInfoBL : BaseBL<PaymentInfo>, IPaymentInfoBL
    {
        public PaymentInfoBL(IDatabaseService databaseService) : base(databaseService)
        {
        }
    }
}
