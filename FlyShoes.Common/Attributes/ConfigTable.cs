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

        public List<string> RelatedTables { get; set; }

        public ConfigTable(string tableName = "",string fieldSearch = "",bool isMaster = false,string relatedTables = "") { 
            TableName = tableName;
            IsMaster = isMaster;
            if (!string.IsNullOrWhiteSpace(fieldSearch))
            {
                FieldSearch = fieldSearch.Split(";").ToList();
            }
            if (!string.IsNullOrWhiteSpace(relatedTables))
            {
                RelatedTables = relatedTables.Split(";").ToList();
            }
        }
    }
}
