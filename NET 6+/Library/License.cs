namespace SoftwareCopyrightManagementSystem;

using System.Security;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Grpc.Core;
public class License
{
    private const string credentialPath = "/config.json";
    private string projectId = String.Empty;
    private FirestoreDb firestoreDb;
    public void ConnectToFirebase()
    {
        var credentialPath = "/config.json";
        GoogleCredential credential = GoogleCredential.FromFile(credentialPath);
        using (Stream stream = new FileStream(credentialPath, FileMode.Open, FileAccess.Read))
        {
            ServiceAccountCredential serviceAccountCredential = ServiceAccountCredential.FromServiceAccountData(stream);
            projectId = serviceAccountCredential.ProjectId;
        }

        string v = FirestoreClient.DefaultEndpoint.ToString();
        firestoreDb = FirestoreDb.Create(projectId, new FirestoreClientBuilder
        {
            ChannelCredentials = ChannelCredentials.Insecure,
            Endpoint = v
        }.Build());
    }
    public async Task<(bool, string)> Check(string collection, string appId, string version = "")
    {
        const string idKey = "id";
        const string licenseTypeKey = "type";
        const string versionKey = "version";

        ConnectToFirebase();

        CollectionReference collectionRef = firestoreDb.Collection(collection);
        Query query = collectionRef.WhereEqualTo(idKey, appId);
        QuerySnapshot querySnapshot = await query.GetSnapshotAsync();

        if (querySnapshot.Documents.Count == 0)
        {
            return (false, Messages.Ufr);
        }

        if (string.IsNullOrEmpty(version))
        {
            return (true, querySnapshot.Documents[0].GetValue<string>(licenseTypeKey));
        }
        
        DocumentSnapshot documentSnapshot = querySnapshot.Documents[0];
        Dictionary<string, object> data = documentSnapshot.ToDictionary();

        if (data.ContainsKey(versionKey) && data[versionKey].ToString() == version)
        {
            return (true, documentSnapshot.GetValue<string>(licenseTypeKey));
        }
        return (false, Messages.LackVersion);
    }

}
