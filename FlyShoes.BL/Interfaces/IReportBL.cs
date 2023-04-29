using FlyShoes.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyShoes.BL.Interfaces
{
    public interface IReportBL
    {
        public Task<List<object>> GetReport(TabAnalyst tab,TimeToAnalyst timeAnalyst);

        public Task<object> AnalystToDay();
    }
}
