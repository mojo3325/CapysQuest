using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Firestore;
using UnityEngine;
using Random = UnityEngine.Random;

public class User
{
    public string email;
    public string referralCode;
}

public class AccountScreenController : MonoBehaviour
{
    public static event Action<AuthenticationState> AuthenticationChecked;
    public static event Action OperationCalled;
    public static event Action<Status, string> OperationFinished;

    private string _userEmail;
    private string _userReferralCode;

    public string UserRefferalCode => _userReferralCode;
    public string UserEmail => _userEmail;

    private Firebase.Auth.FirebaseAuth _auth;
    private FirebaseFirestore _firestore;
    

    private void Start()
    {
        _auth = FirebaseAuth.DefaultInstance;
        _firestore = FirebaseFirestore.DefaultInstance;
    }

    public void CheckAuth()
    {
        if (_auth.CurrentUser != null)
            AuthenticationChecked?.Invoke(AuthenticationState.TRUE);
        else
            AuthenticationChecked?.Invoke(AuthenticationState.FALSE);
    }
    private async Task GetUserDetails()
    {
        DocumentReference docRef = _firestore.Collection("users").Document(_auth.CurrentUser.UserId);
        try
        {
            await docRef.GetSnapshotAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    OperationFinished?.Invoke(Status.Failed, task.Exception?.Message);
                    return;
                }

                var snapshot = task.Result;
                if (snapshot != null && snapshot.Exists)
                {
                    var data = snapshot.ToDictionary();
                    _userEmail = data["email"].ToString();
                    _userReferralCode = data["referralCode"].ToString();
                }
                else
                {
                    OperationFinished?.Invoke(Status.Failed, task.Exception?.Message);
                }
            });
        }
        catch
        {
            OperationFinished?.Invoke(Status.Failed, "Не удалось получить данные аккаунта(");
        }

    }
    
    public async Task SignIn(string email, string password)
    {
        OperationCalled?.Invoke();
        
        await _auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(async task =>
        {
            if (task.IsCanceled)
            {
                OperationFinished?.Invoke(Status.Failed, task.Exception?.Message);
                return;
            }
            if (task.IsFaulted)
            {
                OperationFinished?.Invoke(Status.Failed, task.Exception?.Message);
                return;
            }

            OperationFinished?.Invoke(Status.Success, null);
            await GetUserDetails();
        });
    }
    
    public async Task SignInWithGoogle()
    {
        OperationCalled?.Invoke();
        var credential = GoogleAuthProvider.GetCredential("273831877779-jl021pt5ivg5gg1stqse7u4biaa80m96.apps.googleusercontent.com", null);

        await _auth.SignInWithCredentialAsync(credential).ContinueWith(async task =>
        {
            if (task.IsCanceled)
            {
                OperationFinished?.Invoke(Status.Failed, task.Exception?.Message);
                return;
            }
            if (task.IsFaulted)
            {
                OperationFinished?.Invoke(Status.Failed, task.Exception?.Message);
                return;
            }
            
            var docRef = _firestore.Collection("users").Document(_auth.CurrentUser.UserId);
            
            await docRef.GetSnapshotAsync().ContinueWith(async getTask =>
            {
                if (getTask.IsFaulted)
                {
                    OperationFinished?.Invoke(Status.Failed, task.Exception?.Message);
                    return;
                }

                var snapshot = getTask.Result;
                
                if (snapshot.Exists)
                {
                    _userEmail = snapshot.GetValue<string>("email");
                    _userReferralCode = snapshot.GetValue<string>("referralCode");
                }
                else
                {
                    var refCode = GenerateReferralCode();
                    var newUser = new User
                    {
                        email = _auth.CurrentUser.Email,
                        referralCode = refCode
                    };

                    await docRef.SetAsync(newUser).ContinueWith(setTask =>
                    {
                        if (setTask.IsFaulted)
                        {
                            OperationFinished?.Invoke(Status.Failed, task.Exception?.Message);
                            return;
                        }
                        _userEmail = newUser.email;
                        _userReferralCode = newUser.referralCode;
                    });
                }
            });
        });
    }
    
    public async Task SignUpWithEmailAndPassword(string email, string password)
    {
        OperationCalled?.Invoke();

       await _auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(async task =>
        {
            if (task.IsCanceled)
            {
                OperationFinished?.Invoke(Status.Failed, task.Exception?.Message);
                return;
            }
            if (task.IsFaulted)
            {
                OperationFinished?.Invoke(Status.Failed, task.Exception?.Message);
                return;
            }

            var newUser = task.Result;
            
            var referralCode = GenerateReferralCode();
            
            await SaveUserInfo(newUser.UserId, referralCode, newUser.Email, completed =>
            {
                if (completed)
                {
                    OperationFinished?.Invoke(Status.Success, null);
                }
                else
                {
                    OperationFinished?.Invoke(Status.Failed, task.Exception?.Message);
                }
            });
            
        });
    }
    private static string GenerateReferralCode()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var referralCode = "";
        for (var i = 0; i < 8; i++)
        {
            referralCode += chars[Random.Range(0, chars.Length)];
        }
        return referralCode;
    }
    private static async Task SaveUserInfo(string uid, string referralCode, string email, Action<bool> completion)
    {
        var docRef = FirebaseFirestore.DefaultInstance.Collection("users").Document(uid);
        var data = new Dictionary<string, object>
        {
            { "referralCode", referralCode },
            { "email", email }
        };
        await docRef.SetAsync(data, SetOptions.MergeAll).ContinueWith(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                completion(false);
            }
            if(task.IsCompletedSuccessfully)
            {
                completion(true);
            }
        });
    }
    
    private async void CheckReferralCode(string referralCode)
    {
        var snapshot = await FirebaseFirestore.DefaultInstance.Collection("users").WhereEqualTo("referralCode", referralCode).GetSnapshotAsync();
        if (snapshot.Count == 1)
        {
            var docSnapshot = snapshot.Documents.FirstOrDefault();
            var referredUserId = docSnapshot.Id;
            var docRef = FirebaseFirestore.DefaultInstance.Collection("users").Document(referredUserId);
            var data = new Dictionary<string, object>
            {
                { "referredUsers", FieldValue.ArrayUnion(_auth.CurrentUser.Email) },
                { "referralCount", FieldValue.Increment(1) }
            };
            await docRef.UpdateAsync(data);
        }
    }
    
    private async void GetReferredUsers()
    {
        var docRef = FirebaseFirestore.DefaultInstance.Collection("users").Document(_auth.CurrentUser.UserId);
        var snapshot = await docRef.GetSnapshotAsync();
        if (snapshot.Exists)
        {
            var data = snapshot.ToDictionary();
            if (data.ContainsKey("referredUsers"))
            {
                var referredUsers = (List<object>)data["referredUsers"];
                Debug.Log("Referred Users:");
                foreach (string userId in referredUsers)
                {
                    Debug.Log(userId);
                }
            }
            if (data.ContainsKey("referralCount"))
            {
                var referralCount = (int)(long)data["referralCount"];
                Debug.Log("Referral Count: " + referralCount);
            }
        }
    }
}