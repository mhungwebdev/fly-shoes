using FlyShoes.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FlyShoes.Common
{
    [AttributeUsage(AttributeTargets.Class,AllowMultiple = false)]
    public class ConfigTable : Attribute
    {
        public string TableName { get; set; }

        public List<string> FieldSearch { get; set; }

        public bool IsMaster { get; set; }

        public List<string> DetailTables { get; set; }

        public ConfigTable(string tableName = "",string fieldSearch = "",bool isMaster = false,string detailTables = "") { 
            TableName = tableName;
            IsMaster = isMaster;
            if (!string.IsNullOrWhiteSpace(fieldSearch))
            {
                FieldSearch = fieldSearch.Split(";").ToList();
            }
            if (!string.IsNullOrWhiteSpace(detailTables))
            {
                DetailTables = detailTables.Split(";").ToList();
            }
        }
    }
}
