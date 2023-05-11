using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AppleAuth;
using AppleAuth.Enums;
using AppleAuth.Interfaces;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using Google;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class FireBaseRepo
{
    public static event Action<List<string>> SignInUserSkinsFetched;
    public static event Action SuccessSignUp;
    public static event Action<User> UserFetched;
    
    private FirebaseAuth auth;
    private FirebaseFirestore firestore;
    private IAppleAuthManager appleAuthManager;
    private GoogleSignInConfiguration configuration;
    
    private static string emailHint = "Введите почту";
    private static string passwordHint = "Введите пароль";
    private static string refCodedHint = "Введите реферальный код";
    
    public FireBaseRepo(FirebaseAuth auth, FirebaseFirestore firestore, IAppleAuthManager appleAuthManager, GoogleSignInConfiguration configuration)
    {
        this.auth = auth;
        this.firestore = firestore;
        this.appleAuthManager = appleAuthManager;
        this.configuration = configuration;
    }
    
    public async Task<TaskResult<User>> GetUserDetails()
    {
        try
        {
            var docRef = firestore.Collection("users").Document(auth.CurrentUser.UserId);

            var snapshot = await docRef.GetSnapshotAsync();
            if (snapshot != null && snapshot.Exists)
            {
                var data = snapshot.ConvertTo<User>();
                var result = new TaskResult<User>(data);
                UserFetched?.Invoke(data);
                return result;
            }
            else
            {
                var result = new TaskResult<User>("Данные не найдены");
                return result;
            }
        }
        catch (AggregateException ex)
        {
            var result = new TaskResult<User>(ex.InnerException.Message);
            return result;
        }
    }
    
    public async Task<TaskResult<User>> SignIn(string email, string password)
    {
        try
        {
            if (string.IsNullOrEmpty(email) || string.Equals(email, emailHint))
            {
                return new TaskResult<User>("e-mail адрес не может быть пустым");
            }
        
            if (string.IsNullOrEmpty(password)|| string.Equals(password, passwordHint))
            {
                return new TaskResult<User>("Пароль не может быть пустым");
            }
            
            var authResult = await auth.SignInWithEmailAndPasswordAsync(email, password);
            var userDetailsTask = await GetUserDetails();

            if (userDetailsTask.IsSuccess)
            {
                var data = userDetailsTask.Value;

                if (data is not null)
                {
                    SignInUserSkinsFetched?.Invoke(data.skins);
                    var taskResult = new TaskResult<User>(data);
                    return taskResult;                    
                }
            }

            if (userDetailsTask.IsFailure)
            {
                var taskResult = new TaskResult<User>("Ошибка получения данных пользователя");
                auth.SignOut();
                return taskResult;
            }
        }
        catch (AggregateException ex)
        {
            var message = GetErrorMessage(ex);
            var taskResult = new TaskResult<User>(errorMessage: message);
            return taskResult;
        }
        return null;
    }

    public void SignOut(List<string> provider = null)
    {
        var authProvider = new List<string>();

        if (provider == null)
        {
            authProvider = GetProvider();
        }
        else
            authProvider = provider;

        foreach (var pro in authProvider)
        {
            Debug.Log("Auth Provider in SignOut Firebase Repo is " + pro);
        }
        if (authProvider.Contains(GoogleAuthProvider.ProviderId))
        {
            GoogleSignIn.DefaultInstance.SignOut();
        }

        auth.SignOut();
    }
    
    public async Task<TaskResult<bool>> SaveUserSkins(List<string> skins)
    {
        try
        {
            var user = new User
            {
                skins = skins
            };

            var docRef = firestore.Collection("users").Document(auth.CurrentUser.UserId);
            await docRef.UpdateAsync(new Dictionary<string, object>
            {
                { "skins", user.skins }
            });                
            return new TaskResult<bool>(true);
        }
        catch (Exception)
        {
            return new TaskResult<bool>("Код активирован ,но не удалось сохранить изменение в облако ");
        }
    }
    
    private static string GetErrorMessage(AggregateException exception)
    {
        foreach (var innerEx in exception.Flatten().InnerExceptions)
        {
            if (innerEx is FirebaseException firebaseEx)
            {
                var errorCode = (AuthError)firebaseEx.ErrorCode;
                return GetErrorMessage(errorCode);
            }
            else
            {
                return exception.InnerException.Message;
            }
        }

        return null;
    }
    
    // public void PerformLoginWithAppleIdAndFirebase(Action<FirebaseUser> firebaseAuthCallback)
    // {
    //     var rawNonce = GenerateRandomString(32);
    //     var nonce = GenerateSHA256NonceFromRawNonce(rawNonce);
    //
    //     var loginArgs = new AppleAuthLoginArgs(
    //         LoginOptions.IncludeEmail | LoginOptions.None,
    //         nonce);
    // }

    public async Task<TaskResult<User>> CallAppleSignIn()
    {
        var rawNonce = GenerateRandomString(32);
        var nonce = GenerateSHA256NonceFromRawNonce(rawNonce);

        var loginArgs = new AppleAuthLoginArgs(
            LoginOptions.IncludeEmail,
            nonce);

        try
        {
            var appleLogin = new TaskCompletionSource<TaskResult<IAppleIDCredential>>();

            appleAuthManager.LoginWithAppleId(
                loginArgs, async credential =>
                {
                    if (credential is IAppleIDCredential appleIdCredential)
                    { 
                        appleLogin.SetResult(new TaskResult<IAppleIDCredential>(appleIdCredential));
                        var userId = appleIdCredential.User;
                        PlayerPrefs.SetString("AppleUserIdKey", userId);
                    }
                },
                error =>
                {
                    appleLogin.SetResult(new TaskResult<IAppleIDCredential>(error.LocalizedFailureReason));
                });
            
            var signInUser = await appleLogin.Task;

            if (signInUser.IsSuccess)
            {
                var task = await PerformAppleSignInCredentials(signInUser.Value, rawNonce);

                if (task.IsSuccess)
                {
                    return new TaskResult<User>(task.Value);
                }
                
                if (signInUser.IsFailure)
                {
                    return new TaskResult<User>(signInUser.ErrorMessage);
                }
            }

            if (signInUser.IsFailure)
            {
                return new TaskResult<User>(signInUser.ErrorMessage);
            }
        }
        catch(Exception ex)
        {
            return new TaskResult<User>(ex.Message);
        }

        return null;
    }

    private async Task<TaskResult<User>> PerformAppleSignInCredentials(IAppleIDCredential appleIdCredential, string rawNonce)
    {
        try
        {
            var identityToken = Encoding.UTF8.GetString(appleIdCredential.IdentityToken);
            var authorizationCode = Encoding.UTF8.GetString(appleIdCredential.AuthorizationCode);
            var firebaseCredential = OAuthProvider.GetCredential(
                "apple.com",
                identityToken,
                rawNonce,
                authorizationCode);
            
            var task = await PerformFirebaseAuthWithCredential(firebaseCredential);

            if (task.IsSuccess)
            {
                return new TaskResult<User>(task.Value);
            }

            if (task.IsFailure)
            {
                return new TaskResult<User>(task.ErrorMessage);
            }
            
        }
        catch (Exception ex)
        {
            return new TaskResult<User>(ex.Message);
        }

        return null;
    }

    public async Task<TaskResult<List<string>>> DeleteAccount()
    {
        try
        {
            if (auth.CurrentUser != null)
            {
                var currentAuthProvider = GetProvider();
                
                var docRef = firestore.Collection("users").Document(auth.CurrentUser.UserId);
                
                await docRef.DeleteAsync();
                
                await auth.CurrentUser.DeleteAsync();

                foreach (var info in currentAuthProvider)
                {
                    Debug.Log("CurrentAuthProvider in DeleteAccount is " + info);    
                }
                
                return new TaskResult<List<string>>(value: currentAuthProvider);
            }
            else
            {
                return new TaskResult<List<string>>(errorMessage: "Произошла ошибка при удалении");
            }
        }
        catch(Exception ex)
        {
            return new TaskResult<List<string>>(errorMessage: "Произошла ошибка при удалении");
        }

    }

    public List<string> GetProvider()
    {
        var user = auth.CurrentUser;

        if (user == null) return null;

        var authProviders = new List<string>();
        
        foreach (var info in user.ProviderData)
        {
            authProviders.Add(info.ProviderId);
        }

        return authProviders;
    }
    

    private static string GetErrorMessage(AuthError errorCode)
    {
        var message = errorCode switch
        {
            AuthError.AccountExistsWithDifferentCredentials => "Учетная запись уже существует с другими учетными данными",
            AuthError.MissingPassword => "Требуется пароль",
            AuthError.WrongPassword => "Неправильный пароль",
            AuthError.InvalidEmail => "Недопустимый адрес электронной почты",
            AuthError.MissingEmail => "Требуется адрес электронной почты",
            AuthError.UserNotFound => "Пользователь с таким адресом электронной почты не найден",
            AuthError.WeakPassword => "Слабый пароль",
            AuthError.EmailAlreadyInUse => "Адрес электронной почты уже используется",
            AuthError.UserDisabled => "Учетная запись отключена",
            AuthError.TooManyRequests => "Слишком много запросов. Попробуйте позже",
            AuthError.InvalidCredential => "Недействительные учетные данные",
            AuthError.InvalidUserToken => "Недействительный токен пользователя",
            AuthError.NetworkRequestFailed => "Сбой сети. Попробуйте позже",
            AuthError.EmailChangeNeedsVerification => "Требуется подтверждение изменения адреса электронной почты",
            AuthError.RequiresRecentLogin => "Требуется повторная авторизация",
            AuthError.UserTokenExpired => "Срок действия токена пользователя истек",
            AuthError.InvalidApiKey => "Недействительный ключ API",
            AuthError.InvalidRecipientEmail => "Недопустимый адрес электронной почты получателя",
            _ => "Произошла ошибка аутентификации"
        };

        return message;
    }
    
    public async Task<TaskResult<User>> PerformFirebaseAuthWithCredential(Credential credential)
    {
        try
        {
            var authResult = await auth.SignInWithCredentialAsync(credential);
        
            var docRef = firestore.Collection("users").Document(authResult.UserId);

            var snapshot = await docRef.GetSnapshotAsync();

            if (snapshot.Exists)
            {
                var userDetailsTask = await GetUserDetails();

                if (userDetailsTask.IsSuccess)
                {
                    SignInUserSkinsFetched?.Invoke(userDetailsTask.Value.skins);
                    var result = new TaskResult<User>(userDetailsTask.Value);
                    return result;
                }
                else
                {
                    return new TaskResult<User>(userDetailsTask.ErrorMessage);
                }
            }
            else
            {
                var refCode = GenerateReferralCode();
                var newUser = new User
                {
                    userID = (string.IsNullOrEmpty(authResult.Email) ? authResult.UserId : authResult.Email),
                    referralCode = refCode
                };

                var saveInfoTask = await SaveUserInfo(uid: authResult.UserId,referralCode: newUser.referralCode, userId: newUser.userID);

                if (saveInfoTask.IsSuccess)
                {
                    var result = new TaskResult<User>(newUser);
                    SuccessSignUp?.Invoke();
                    return result;
                }
                if (saveInfoTask.IsFailure)
                {
                    var result = new TaskResult<User>(saveInfoTask.ErrorMessage);    
                    return result;
                }
            }
        }
        catch (AggregateException ex)
        {
            var message = GetErrorMessage(ex);
            var taskResult = new TaskResult<User>(errorMessage: message);
            return taskResult;
        }

        return null;
    }
    public async Task<TaskResult<User>> SignUpWithEmailAndPassword(string email, string password)
    {
        try
        {
            if (string.IsNullOrEmpty(email) || string.Equals(email, emailHint))
            {
                return new TaskResult<User>("e-mail адрес не может быть пустым");
            }
        
            if (string.IsNullOrEmpty(password)|| string.Equals(password, passwordHint))
            {
                return new TaskResult<User>("Пароль не может быть пустым");
            }

            var signUpTask = await auth.CreateUserWithEmailAndPasswordAsync(email, password);
            var referralCode = GenerateReferralCode();
            var saveInfoTask = await SaveUserInfo(signUpTask.UserId, referralCode, signUpTask.Email);

            if (saveInfoTask.IsSuccess)
            {
                var data = new User
                {
                    userID = email,
                    referralCode = referralCode
                };
            
                var result = new TaskResult<User>(data);
                SuccessSignUp?.Invoke();
                return result;
            }

            if (saveInfoTask.IsFailure)
            {
                var result = new TaskResult<User>(saveInfoTask.ErrorMessage);    
                auth.SignOut();
                return result;
            }
        }
        catch (AggregateException ex)
        {
            var message = GetErrorMessage(ex);
            var taskResult = new TaskResult<User>(errorMessage: message);
            return taskResult;
        }

        return null;
    }
    
    private string GenerateReferralCode()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var referralCode = "";
        for (var i = 0; i < 8; i++)
        {
            referralCode += chars[Random.Range(0, chars.Length)];
        }
        return referralCode;
    }
    
    private async Task<TaskResult<bool>> SaveUserInfo(string uid, string referralCode, string userId)
    {
        var docRef = firestore.Collection("users").Document(uid);

        var data = new User
        {
            userID = userId,
            referralCode = referralCode
        };

        try
        {
            await docRef.SetAsync(data, SetOptions.MergeAll);
            return new TaskResult<bool>(true);
        }
        catch (AggregateException ex)
        {
            var message = GetErrorMessage(ex);
            return new TaskResult<bool>(message);
        }
    }

    public async Task<TaskResult<bool>> EnterReferralCode(string referralCode)
    {
        try
        {
            if (string.IsNullOrEmpty(referralCode) || string.Equals(referralCode, refCodedHint))
            {
                return new TaskResult<bool>("Реферальный код не может быть пустым");
            }
            
            var snapshot = await FirebaseFirestore.DefaultInstance.Collection("users")
                .WhereEqualTo("referralCode", referralCode)
                .GetSnapshotAsync();

            if (snapshot.Count == 1)
            {
                var referredUserDocSnapshot = snapshot.Documents.FirstOrDefault();
                var referredUserId = referredUserDocSnapshot.Id;

                var referredUserDocRef = FirebaseFirestore.DefaultInstance.Collection("users")
                    .Document(referredUserId);

                var referredUserData = new Dictionary<string, object>
                {
                    {"referredUsers", FieldValue.ArrayUnion(auth.CurrentUser.Email)},
                    {"finishBonus", FieldValue.Increment(1)},
                };

                await referredUserDocRef.UpdateAsync(referredUserData);

                var currentUserDocRef = FirebaseFirestore.DefaultInstance.Collection("users")
                    .Document(auth.CurrentUser.UserId);
                var currentUserDocSnapshot = await currentUserDocRef.GetSnapshotAsync();

                var currentUserData = new Dictionary<string, object>
                {
                    {"enteredReferralCode", referralCode}
                };

                await currentUserDocRef.UpdateAsync(currentUserData);

                return new TaskResult<bool>(true);
            }
            else
            {
                return new TaskResult<bool>("Такого реферального кода не существует");
            }
        }
        catch (Exception ex)
        {
            return new TaskResult<bool>(ex.Message);
        }
    }

    private static string GenerateSHA256NonceFromRawNonce(string rawNonce)
    {
        var sha = new SHA256Managed();
        var utf8RawNonce = Encoding.UTF8.GetBytes(rawNonce);
        var hash = sha.ComputeHash(utf8RawNonce);

        var result = string.Empty;
        for (var i = 0; i < hash.Length; i++)
        {
            result += hash[i].ToString("x2");
        }

        return result;
    }
    
    private static string GenerateRandomString(int length)
    {
        if (length <= 0)
        {
            throw new Exception("Expected nonce to have positive length");
        }

        const string charset = "0123456789ABCDEFGHIJKLMNOPQRSTUVXYZabcdefghijklmnopqrstuvwxyz-._";
        var cryptographicallySecureRandomNumberGenerator = new RNGCryptoServiceProvider();
        var result = string.Empty;
        var remainingLength = length;

        var randomNumberHolder = new byte[1];
        while (remainingLength > 0)
        {
            var randomNumbers = new List<int>(16);
            for (var randomNumberCount = 0; randomNumberCount < 16; randomNumberCount++)
            {
                cryptographicallySecureRandomNumberGenerator.GetBytes(randomNumberHolder);
                randomNumbers.Add(randomNumberHolder[0]);
            }

            for (var randomNumberIndex = 0; randomNumberIndex < randomNumbers.Count; randomNumberIndex++)
            {
                if (remainingLength == 0)
                {
                    break;
                }

                var randomNumber = randomNumbers[randomNumberIndex];
                if (randomNumber < charset.Length)
                {
                    result += charset[randomNumber];
                    remainingLength--;
                }
            }
        }

        return result;
    }

    public async Task<TaskResult<bool>> ActivateBonusCode(BonusCode code)
    {
        try
        {
            var userInfoTask = await GetUserDetails();
            
            if (userInfoTask.IsSuccess)
            {
                var activatedCodes = userInfoTask.Value.activatedCodes;

                if (activatedCodes != null)
                {
                    if (activatedCodes.Contains(code.Symbol))
                    {
                        return new TaskResult<bool>("Console Exception :error with current achievement");
                    }
                
                    var documentRef = firestore.Collection("users").Document(auth.CurrentUser.UserId);
                
                    var updateData = new Dictionary<string, object>
                    {
                        { "activatedCodes", FieldValue.ArrayUnion(code.Symbol)},
                        {"finishBonus", FieldValue.Increment(code.Reward)}
                    };
                
                    await documentRef.UpdateAsync(updateData);
                    return new TaskResult<bool>(true);
                }
                else
                {
                    var documentRef = firestore.Collection("users").Document(auth.CurrentUser.UserId);
                
                    var updateData = new Dictionary<string, object>
                    {
                        { "activatedCodes", FieldValue.ArrayUnion(code.Symbol)},
                        {"finishBonus", FieldValue.Increment(code.Reward)}
                    };
                
                    await documentRef.UpdateAsync(updateData);
                    return new TaskResult<bool>(true);
                }
            }
            else
            {
                return new TaskResult<bool>("Console Exception :error with current achievement");
            }
        }
        catch (Exception e)
        {
            return new TaskResult<bool>("Console Exception :error with current achievement");
        }
    }
}