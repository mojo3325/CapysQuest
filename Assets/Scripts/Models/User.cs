
using System.Collections.Generic;
using Firebase.Firestore;
using Newtonsoft.Json;

[FirestoreData]
public class User
{
    [JsonProperty("userID")]
    [FirestoreProperty("userID")]
    public string userID { get; set; }
    
    [JsonProperty("referralCode")]
    [FirestoreProperty("referralCode")]
    public string referralCode { get; set; }
    
    [JsonProperty("enteredReferralCode")]
    [FirestoreProperty("enteredReferralCode")]
    public string enteredReferralCode { get; set; }

    [JsonProperty("referredUsers")]
    [FirestoreProperty("referredUsers")]
    public List<string> referredUsers { get; set; }

    [JsonProperty("finishBonus")]
    [FirestoreProperty("finishBonus")]
    public int finishBonus { get; set; }

    [JsonProperty("skins")]
    [FirestoreProperty("skins")]
    public List<string> skins { get; set; }

    [JsonProperty("activatedCodes")]
    [FirestoreProperty("activatedCodes")]
    public List<string> activatedCodes { get; set; }
}