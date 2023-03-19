using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyShoes.DAL.Interfaces
{
    public interface IFirestoreService
    {
        void PushNotification(int userID,string notificationContent,string reference);
    }
}
