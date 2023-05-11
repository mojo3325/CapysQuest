using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppleAuth;
using AppleAuth.Native;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using Google;
using Unity.VisualScripting;
using UnityEngine;

public class FireBaseController : MonoBehaviour
{
    public static event Action FireBaseRepoInitialized;
    
    private FirebaseAuth _auth;
    private FirebaseFirestore _firestore;
    private FireBaseRepo _fireBaseRepo;
    private IAppleAuthManager appleAuthManager;

    public FirebaseFirestore Firestore => _firestore;
    public FireBaseRepo FireBaseRepo => _fireBaseRepo;
    
    private GoogleSignInConfiguration _configuration;
    private const string WebClientId = "273831877779-qfmbdu203atf9crtnlou3jj5ttrgcuj3.apps.googleusercontent.com";

    private List<BonusCode> _bonusCodesList = new();
    public List<BonusCode> BonusCodeList => _bonusCodesList;
    
    private const string FirstLaunchKey = "FirstLaunch";
    
    private void Update()
    {
        appleAuthManager?.Update();
    }

    private void Start()
    {
        if (AppleAuthManager.IsCurrentPlatformSupported)
        {
            var deserializer = new PayloadDeserializer();
            appleAuthManager = new AppleAuthManager(deserializer);    
        }
    }

    private async Task AuthSync()
    {
        if (_auth != null)
        {
            var isAuthenticated = CheckAuth();
        
            if (isAuthenticated)
            {
                var provider = _fireBaseRepo.GetProvider();
        
                if(provider.Contains(GoogleAuthProvider.ProviderId))
                {
                    GoogleSignIn.Configuration = _configuration;
            
                    if(GoogleSignIn.Configuration != null)
                        await GoogleSignIn.DefaultInstance.SignInSilently();
                }   
            }
        }
    }

    private async void Awake()
    {
        var isFirstLaunch = PlayerPrefs.GetInt(FirstLaunchKey, 1) == 1;

        var task = await FirebaseApp.CheckAndFixDependenciesAsync();

        if (task == DependencyStatus.Available)
        {
            _auth = FirebaseAuth.DefaultInstance;
            _firestore = FirebaseFirestore.DefaultInstance;
            _configuration = new GoogleSignInConfiguration { WebClientId = WebClientId, RequestEmail = true, RequestIdToken = true };
            _fireBaseRepo = 
                new FireBaseRepo(auth: _auth, firestore: _firestore, appleAuthManager: appleAuthManager,configuration: _configuration);

            if (isFirstLaunch)
            {
                StartCoroutine(ClearAuthCache());
            }

            await AuthSync();
            
            FireBaseRepoInitialized?.Invoke();
        }

    }

    public async Task<TaskResult<bool>> SyncBonusCodes()
    {
        try
        {
            var isAuthenticated = CheckAuth();
            
            if (isAuthenticated)
            {
                var docRef = _firestore.Collection("Codes").Document("bonus_codes");
                var snapshot = await docRef.GetSnapshotAsync();

                if (!snapshot.Exists) return new TaskResult<bool>("Не удалось получить бонусные коды");
                
                var codes = snapshot.GetValue<List<object>>("value");

                if (codes == null) return new TaskResult<bool>("Не удалось получить бонусные коды");
                    
                foreach (var codeObj in codes)
                {
                    if (codeObj is Dictionary<string, object> codeDict)
                    {
                        var symbol = codeDict["symbol"] as string;
                        var reward = Convert.ToInt32(codeDict["reward"]);

                        var bonusCode = new BonusCode
                        {
                            Symbol = symbol,
                            Reward = reward
                        };
                                
                        _bonusCodesList.Add(bonusCode);
                    }
                }

                return new TaskResult<bool>(true);
            }
            
            return new TaskResult<bool>("Не удалось получить бонусные коды");
        }
        catch (Exception e)
        {
            return new TaskResult<bool>(e.Message);
        }
    } 
    
    public bool CheckAuth()
    {
        return _auth.CurrentUser != null;
    }
    
    public void SignOut(List<string> provider = null)
    {
        Debug.Log("SignOut Was Called");
        _fireBaseRepo.SignOut(provider);
    }
    
    public async Task<TaskResult<List<string>>> DeleteAccount()
    {
        var task = await _fireBaseRepo.DeleteAccount();

        if (task.IsSuccess)
        {
            Debug.Log("await _fireBaseRepo.DeleteAccount TASK is Success");
            return new TaskResult<List<string>>(task.Value);
        }

        if (task.IsFailure)
        {
            Debug.Log("await _fireBaseRepo.DeleteAccount TASK is Failure");
            return new TaskResult<List<string>>(errorMessage: task.ErrorMessage);
        }
        return null;
    }

    public async Task<TaskResult<User>> SignInWithEmailAndPassword(string email, string password)
    {
        var signInTask = await _fireBaseRepo.SignIn(email: email, password: password);

        if (signInTask.IsSuccess)
        {
            return new TaskResult<User>(signInTask.Value);
        }
    
        if (signInTask.IsFailure)
        {
            return new TaskResult<User>(signInTask.ErrorMessage);
        }

        return null;
    }
    
    public async Task<TaskResult<User>> SignInWithGoogle()
    {
        GoogleSignIn.Configuration = _configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;

        if (GoogleSignIn.Configuration == null)
        {
            _configuration = new GoogleSignInConfiguration
                {WebClientId = WebClientId, RequestEmail = true, RequestIdToken = true};
            GoogleSignIn.Configuration = _configuration;
        }

        try
        {
            var signInTaskSource = new TaskCompletionSource<GoogleSignInUser>();
            
            await GoogleSignIn.DefaultInstance.SignIn().ContinueWith(signInTask => {
                
                if (signInTask.IsCanceled)
                {
                    signInTaskSource.SetCanceled();
                }
                else if (signInTask.IsFaulted)
                {
                    signInTaskSource.SetException(signInTask.Exception);
                }
                else if(signInTask.IsCompletedSuccessfully)
                {
                    signInTaskSource.SetResult(signInTask.Result);
                }
            });
            
            var signInUser = await signInTaskSource.Task;
            
            var idToken = signInUser.IdToken;
            
            var credential = GoogleAuthProvider.GetCredential(idToken, null);

            var task = await _fireBaseRepo.PerformFirebaseAuthWithCredential(credential);

            if (task.IsSuccess)
            {
                return new TaskResult<User>(task.Value);
            }

            if (task.IsFailure)
            {
                SignOut();
                return new TaskResult<User>(task.ErrorMessage);
            }

            return null;
        }
        catch (OperationCanceledException)
        {
            return new TaskResult<User>("Вход с помощью Google был отменен.");
        }
        catch (Exception ex)
        {
            return new TaskResult<User>(ex.InnerException.Message);
        }
    }
    
    public async Task<TaskResult<User>> SignInWithApple()
    {
        try
        {
            var task = await _fireBaseRepo.CallAppleSignIn();

            if (task.IsSuccess)
            {
                return new TaskResult<User>(task.Value);
            }

            if (task.IsFailure)
            {
                SignOut();
                return new TaskResult<User>("Не удалось войти через Apple (");
            }

            return null;
        }
        catch (OperationCanceledException)
        {
            return new TaskResult<User>("Не удалось войти через Apple (.");
        }
        catch (Exception ex)
        {
            return new TaskResult<User>("Не удалось войти через Apple (");
        }
    }
    
    public async Task<TaskResult<User>> SignUpWithEmailAndPassword(string email, string password)
    {
        var signUpTask = await _fireBaseRepo.SignUpWithEmailAndPassword(email: email, password: password);
        
        if (signUpTask.IsSuccess)
        {
            return new TaskResult<User>(signUpTask.Value);
        }
        
        if (signUpTask.IsFailure)
        {
            return new TaskResult<User>(signUpTask.ErrorMessage);
        }

        return null;
    }
    
    public async Task<TaskResult<User>> GetUserDetails()
    {
        var task = await _fireBaseRepo.GetUserDetails();

        if (task.IsSuccess)
        {
            if (!string.IsNullOrEmpty(task.Value.userID) && !string.IsNullOrEmpty(task.Value.referralCode))
            {
                return new TaskResult<User>(task.Value);
            }
            else
            {
                return new TaskResult<User>("Не удалось получить данные");
            }
        }

        if (task.IsFailure)
        {
            return new TaskResult<User>("Не удалось получить данные");
        }

        return null;
    }

    public async Task<TaskResult<bool>> EnterRefCode(string code)
    {
        var task = await _fireBaseRepo.EnterReferralCode(code);
            
        if (task.IsSuccess)
        {
            return new TaskResult<bool>(true);
        }

        if (task.IsFailure)
        {
            return new TaskResult<bool>(task.ErrorMessage);
        }

        return null;
    }

    public async Task<TaskResult<bool>> SaveUserSkins(List<string> skins)
    {
        try
        {
            var isAuthenticated = CheckAuth();

            if (isAuthenticated)
            {
                var task = await _fireBaseRepo.SaveUserSkins(skins);

                if (task.IsSuccess)
                {
                    return new TaskResult<bool>(true);
                }

                if (task.IsFailure)
                {
                    return new TaskResult<bool>(task.ErrorMessage);
                }
            }
            else
            {
                return new TaskResult<bool>(true);
            }
        }
        catch (Exception e)
        {
            return new TaskResult<bool>(e.Message);
        }
        
        return null;
    }

    public async Task<TaskResult<bool>> ActivateBonusCode(BonusCode code)
    {
        var task = await _fireBaseRepo.ActivateBonusCode(code);

        if (task.IsSuccess)
        {
            return new TaskResult<bool>(true);
        }

        if (task.IsFailure)
        {
            return new TaskResult<bool>(task.ErrorMessage);
        }

        return null;
    }
    
    private IEnumerator ClearAuthCache()
    {
        GoogleSignIn.Configuration = _configuration;
        GoogleSignIn.DefaultInstance.SignOut();
        GoogleSignIn.DefaultInstance.Disconnect();
        _auth.SignOut();
        yield return new WaitForSeconds(1f);
        
        if (_auth.CurrentUser == null)
        {
            PlayerPrefs.SetInt(FirstLaunchKey, 0);
        }
    }
}