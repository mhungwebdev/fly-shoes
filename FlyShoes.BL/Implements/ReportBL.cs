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
                    for (var i = -(dayOfWeek - 1); i < -(dayOfWeek - 1) + 7; i++)
                    {
                        var date = now.AddDays(i);
                        switch (date.DayOfWeek)
                        {
                            case DayOfWeek.Monday:
                                param.Add("v_Monday", date.Day);
                                break;
                            case DayOfWeek.Sunday:
                                param.Add("v_Sunday", date.Day);
                                break;
                        }
                    }

                    break;
                case TimeToAnalyst.LastWeek:
                    param.Remove("v_Monday");
                    param.Remove("v_Sunday");
                    var lastWeek = now.AddDays(-7);
                    var dayOfLastWeek = (int)lastWeek.DayOfWeek;
                    for (var i = -(dayOfLastWeek - 1); i < -(dayOfLastWeek - 1) + 7; i++)
                    {
                        var date = lastWeek.AddDays(i);
                        switch (date.DayOfWeek)
                        {
                            case DayOfWeek.Monday:
                                param.Add("v_Monday", date.Day);
                                break;
                            case DayOfWeek.Sunday:
                                param.Add("v_Sunday", date.Day);
                                break;
                        }
                    }
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
    }
}
