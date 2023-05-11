using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class AccountScreenController : MonoBehaviour
{
    public static event Action UserSignedOut;

    [Header("Controllers")]
    [SerializeField]
    private FireBaseController _fireBaseController;
    
    private User _currentUser;
    private int _finishPrize;
    private bool _dataLoaded;
    
    public User CurrentUser => _currentUser;
    public int FinishPrize => _finishPrize;
    public List<string> Skinslist => _currentUser.skins;

    public bool CheckAuth()
    {
        if (_fireBaseController != null)
        {
            var state = _fireBaseController.CheckAuth();
            return state;
        }

        return false;
    }

    public void SignOut(List<string> provider = null)
    {
        if (_fireBaseController != null)
        {
            _fireBaseController.SignOut(provider);
            _currentUser = new User();
            UserSignedOut?.Invoke();
        }
    }

    public async Task<TaskResult<bool>> DeleteAccount()
    {
        OperationEvent.OnMethodCalled();

        if (_fireBaseController != null)
        {
            var task = await _fireBaseController.DeleteAccount();

            if (task.IsSuccess)
            {
                Debug.Log("await _fireBaseController.DeleteAccount TASK is Success");
                Debug.Log("_fireBaseController.DeleteAccount Task Value is " + task.Value);
                SignOut(provider: task.Value);
                OperationEvent.OnMethodFinished(new TaskResult<bool>(true));
                return new TaskResult<bool>(true);
            }

            if (task.IsFailure)
            {
                Debug.Log("await _fireBaseController.DeleteAccount TASK is failure");
                OperationEvent.OnMethodFinished(new TaskResult<bool>(task.ErrorMessage));
                return new TaskResult<bool>(task.ErrorMessage);
            }

            return null;
        }
        else
        {
            OperationEvent.OnMethodFinished(new TaskResult<bool>("Не удалось удалить аккаунт"));
            return new TaskResult<bool>("Не удалось удалить аккаунт");
        }
    }
    
    public async Task<TaskResult<bool>> SignInWithEmailAndPassword(string email, string password)
    {
        OperationEvent.OnMethodCalled();

        if (_fireBaseController != null)
        {
            
            var signInTask = await _fireBaseController.SignInWithEmailAndPassword(email: email, password: password);

            if (signInTask.IsSuccess)
            {
                _currentUser = signInTask.Value;
                OperationEvent.OnMethodFinished(new TaskResult<bool>(true));
                return new TaskResult<bool>(true);
            }
    
            if (signInTask.IsFailure)
            {
                OperationEvent.OnMethodFinished(new TaskResult<bool>(signInTask.ErrorMessage));
                return new TaskResult<bool>(signInTask.ErrorMessage);
            }

            return null;    
        }
        else
        {
            OperationEvent.OnMethodFinished(new TaskResult<bool>("Не удалось войти в аккаунт"));
            return new TaskResult<bool>("Не удалось войти в аккаунт");
        }
    }

    public async Task<TaskResult<bool>> SignInWithGoogle()
    {
        OperationEvent.OnMethodCalled();

        if (_fireBaseController != null)
        {
            var task = await _fireBaseController.SignInWithGoogle();

                if (task.IsSuccess)
                {
                    Debug.Log(" SignInWithGoogle Task is Success");
                    _currentUser = task.Value;
                    OperationEvent.OnMethodFinished(new TaskResult<bool>(true));
                    return new TaskResult<bool>(true);
                }

                if (task.IsFailure)
                {
                    Debug.Log(" SignInWithGoogle Task is Failure");
                    OperationEvent.OnMethodFinished(new TaskResult<bool>(task.ErrorMessage));
                    return new TaskResult<bool>(task.ErrorMessage);
                }

                return null;
        }
        else
        {
            OperationEvent.OnMethodFinished(new TaskResult<bool>("Не удалось войти с помощью Google"));
            return new TaskResult<bool>("Не удалось войти с помощью Google");
        }   
    }

    public async Task<TaskResult<bool>> SignInWithApple()
    {
        OperationEvent.OnMethodCalled();

        if (_fireBaseController != null)
        {
            var task = await _fireBaseController.SignInWithApple();

            if (task.IsSuccess)
            {
                _currentUser = task.Value;
                OperationEvent.OnMethodFinished(new TaskResult<bool>(true));
                return new TaskResult<bool>(true);
            }

            if (task.IsFailure)
            {
                OperationEvent.OnMethodFinished(new TaskResult<bool>("Не удалось войти через Apple ("));
                return new TaskResult<bool>("Не удалось войти через Apple (");
            }
        }
        else
        {
            OperationEvent.OnMethodFinished(new TaskResult<bool>("Не удалось войти с помощью Google"));
            return new TaskResult<bool>("Не удалось войти с помощью Google");
        }

        return null;
    }
    
    public async Task<TaskResult<bool>> SignUpWithEmailAndPassword(string email, string password)
    {
        OperationEvent.OnMethodCalled();

        if (_fireBaseController != null)
        {
            var signUpTask = await _fireBaseController.SignUpWithEmailAndPassword(email: email, password: password);
        
            if (signUpTask.IsSuccess)
            {
                _currentUser = signUpTask.Value;
                OperationEvent.OnMethodFinished(new TaskResult<bool>(true));
                return new TaskResult<bool>(true);
            }
        
            if (signUpTask.IsFailure)
            {
                OperationEvent.OnMethodFinished(new TaskResult<bool>(signUpTask.ErrorMessage));
                return new TaskResult<bool>(signUpTask.ErrorMessage);
            }
        }
        else
        {
            OperationEvent.OnMethodFinished(new TaskResult<bool>("Не удалось войти с помощью Google"));
            return new TaskResult<bool>("Не удалось войти с помощью Google");
        }

        return null;
    }

    public async Task<Status?> GetUserDetails()
    {
        if (_fireBaseController != null)
        {
            var task = await _fireBaseController.GetUserDetails();
            
            if (task.IsSuccess)
            {
                _currentUser = task.Value;
                _finishPrize = 100 + _currentUser.finishBonus;
                return Status.Success;
            }

            if (task.IsFailure)
            {
                return Status.Failure;
            }
        }

        return null;
    }

    
    public async Task<TaskResult<bool>> EnterRefCode(string code)
    {
        OperationEvent.OnMethodCalled();

        if (_fireBaseController != null)
        {
            if (!string.Equals(code, _currentUser.referralCode))
            {
                var task = await _fireBaseController.EnterRefCode(code);
            
                if (task.IsSuccess)
                {
                    OperationEvent.OnMethodFinished(new TaskResult<bool>(true));
                    return new TaskResult<bool>(true);
                }

                if (task.IsFailure)
                {
                    OperationEvent.OnMethodFinished(new TaskResult<bool>(task.ErrorMessage));
                    return new TaskResult<bool>(task.ErrorMessage);
                }
            }
            else
            {
                OperationEvent.OnMethodFinished(new TaskResult<bool>("Нельзя использовать собственный код)"));
                return new TaskResult<bool>(("Нельзя использовать собственный код)"));
            }
        }
        
        return null;
    }
}
