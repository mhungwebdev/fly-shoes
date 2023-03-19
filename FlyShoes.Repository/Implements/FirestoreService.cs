using Firebase.Database;
using Firebase.Database.Query;
using FlyShoes.DAL.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyShoes.DAL.Implements
{
    public class FirestoreService : IFirestoreService
    {
        private FirebaseClient _firebaseClient;

        public FirestoreService(IConfiguration configuration)
        {
            _firebaseClient = new FirebaseClient(configuration.GetSection("FirebaseDatabase").Value);
        }

        public void PushNotification(int userID, string notificationContent, string reference)
        {
            var notification = new { 
                UserID = userID,
                Content = notificationContent,
                Reference = reference
            };

            var res = _firebaseClient.Child("Notification").PatchAsync(notification);
        }
    }
}
