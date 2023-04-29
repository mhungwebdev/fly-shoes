using FlyShoes.BL.Interfaces;
using FlyShoes.Common.Enums;
using FlyShoes.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyShoes.BL.Implements
{
    public class ReportBL : IReportBL
    {
        IDatabaseService _databaseService;
        public ReportBL(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public async Task<List<object>> GetReport(TabAnalyst tab, TimeToAnalyst timeAnalyst)
        {
            var param = GetParamReport(tab, timeAnalyst);
            var data = new List<object>();

            switch (tab)
            {
                case TabAnalyst.Order:
                    data = _databaseService.QueryUsingStoredProcedure<object>("Proc_Report_Order", param);
                    break;
                case TabAnalyst.Product:
                    data = _databaseService.QueryUsingStoredProcedure<object>("Proc_Report_Product", param);
                    break;
                case TabAnalyst.InCome:
                    data = _databaseService.QueryUsingStoredProcedure<object>("Proc_Report_InCome", param);
                    break;
                case TabAnalyst.Customer:
                    data = _databaseService.QueryUsingStoredProcedure<object>("Proc_Report_Customer", param);
                    break;
            }

            return data;
        }

        public Dictionary<string, object> GetParamReport(TabAnalyst tab, TimeToAnalyst timeAnalyst)
        {
            var now = DateTime.Now;
            var param = new Dictionary<string, object>();
            param.Add("v_Month", now.Month);
            param.Add("v_Year", now.Year);
            param.Add("v_TimeAnalyst", timeAnalyst);
            param.Add("v_Monday", 0);
            param.Add("v_Sunday", 0);

            switch (timeAnalyst)
            {
                case TimeToAnalyst.CurrentWeek:
                    var dayOfWeek = (int)now.DayOfWeek;
                    param.Remove("v_Monday");
                    param.Remove("v_Sunday");
                    var numberMonday = dayOfWeek == (int)DayOfWeek.Sunday ? -6 : dayOfWeek - 1;
                    var numberSunday = dayOfWeek == (int)DayOfWeek.Sunday ? 0 : 7 - dayOfWeek;
                    param.Add("v_Monday", now.AddDays(numberMonday).Day);
                    param.Add("v_Sunday", now.AddDays(numberSunday).Day);
                    break;
                case TimeToAnalyst.LastWeek:
                    param.Remove("v_Monday");
                    param.Remove("v_Sunday");
                    var lastWeek = now.AddDays(-7);
                    var dayOfLastWeek = (int)lastWeek.DayOfWeek;
                    var numberMondayLastWeek = dayOfLastWeek == (int)DayOfWeek.Sunday ? -6 : dayOfLastWeek - 1;
                    var numberSundayLastWeek = dayOfLastWeek == (int)DayOfWeek.Sunday ? 0 : 7 - dayOfLastWeek;
                    param.Add("v_Monday", lastWeek.AddDays(numberMondayLastWeek).Day);
                    param.Add("v_Sunday", lastWeek.AddDays(numberSundayLastWeek).Day);
                    break;
                case TimeToAnalyst.LastMonth:
                    param.Remove("v_Month");
                    param.Add("v_Month", now.Month == 1 ? 12 : now.Month - 1);
                    break;
                case TimeToAnalyst.LastYear:
                    param.Remove("v_Year");
                    param.Add("v_Year", now.Year - 1);
                    break;
            }

            return param;
        }

        public async Task<object> AnalystToDay()
        {
            var res = await _databaseService.QueryUsingStoredProcedureAsync("Proc_Report_Today");

            return res.FirstOrDefault();
        }
    }
}
