using Firebase.Firestore;
using Newtonsoft.Json;

[FirestoreData]
public class BonusCode
{
    [JsonProperty("symbol")]
    [FirestoreProperty("symbol")]
    public string Symbol { get; set; }

    [JsonProperty("reward")]
    [FirestoreProperty("reward")]
    public int Reward { get; set; }
}
