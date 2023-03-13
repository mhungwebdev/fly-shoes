using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyShoes.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Class,AllowMultiple = false)]
    public class ConfigTable : Attribute
    {
        private string TableName { get; set; }

        public ConfigTable(string tableName = "") { 
            this.TableName = tableName;
        }
    }
}
