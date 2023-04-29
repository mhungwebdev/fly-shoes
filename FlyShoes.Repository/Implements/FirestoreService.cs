using Firebase.Database;
using Firebase.Database.Query;
using FirebaseAdmin;
using FlyShoes.Common.Models;
using FlyShoes.DAL.Interfaces;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyShoes.DAL.Implements
{
    public class FirestoreService : IFirestoreService
    {
        private FirestoreDb _firebaseClient;
        private CollectionReference _notification;

        public FirestoreService(IConfiguration configuration)
        {
            FirestoreClientBuilder builder = new FirestoreClientBuilder();
            builder.Credential = Google.Apis.Auth.OAuth2.GoogleCredential.FromFile("./FirebaseConfig/firebase-config.json");

            _firebaseClient = FirestoreDb.Create("fly-shoes-store",builder.Build());
            _notification = _firebaseClient.Collection("notification");
        }

        public async Task PushNotification(Notification notification)
        {
            var serializedParticipant = JsonConvert.SerializeObject(notification);
            var deserializedParticipant = JsonConvert.DeserializeObject<ExpandoObject>(serializedParticipant);

            await _notification.AddAsync(deserializedParticipant);
        }
    }
}
