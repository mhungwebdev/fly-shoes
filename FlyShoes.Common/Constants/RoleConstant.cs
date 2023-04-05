using FlyShoes.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyShoes.Common
{
    public class RoleConstant
    {
        public static List<Type> MODEL_ALLOW_CUSTOMER_DELETE = new List<Type>() { typeof(CartDetail)};
        public static List<Type> MODEL_ALLOW_CUSTOMER_DELETE_MULTI = new List<Type>() { typeof(CartDetail)};
        public static List<Type> MODEL_ALLOW_CUSTOMER_INSERT = new List<Type>() { typeof(CartDetail),typeof(OrderShoes),typeof(OrderDetail) };
        public static List<Type> MODEL_ALLOW_CUSTOMER_UPDATE = new List<Type> { typeof(CartDetail)};
    }
}
