namespace SoftwareCopyrightManagementSystem;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Grpc.Core;

public static class FirebaseHelper
{
    public static async Task<FirestoreDb> CreateFirestoreDb(string credentialPath)
    {
        var credential = GoogleCredential.FromFile(credentialPath);
        using (Stream stream = new FileStream(credentialPath, FileMode.Open, FileAccess.Read))
        {
            var serviceAccountCredential = ServiceAccountCredential.FromServiceAccountData(stream);
            var projectId = serviceAccountCredential.ProjectId;
            var v = FirestoreClient.DefaultEndpoint.ToString();
            var firestoreDb = FirestoreDb.Create(projectId, new FirestoreClientBuilder
            {
                ChannelCredentials = ChannelCredentials.Insecure,
                Endpoint = v
            }.Build());

            return await Task.FromResult(firestoreDb);
        }
    }
}