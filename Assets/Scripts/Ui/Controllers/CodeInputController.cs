using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class CodeInputController : MonoBehaviour
{
    [Header("Controllers")]
    [SerializeField] private AccountScreenController _accountScreenController;
    [SerializeField] private CapyController _capyController;
    [SerializeField] private FireBaseController _fireBaseController;
    
    private string[] _skinCodeList = {"A3SK1N", "SUP3RSK1N", "M0US3SKIN", "F0XSKIN", "RA1NB0WSK1N", "BUS1NESS_SK1N"};

    public static event Action CodeEnteredSuccess;
    private static string codeHint = "Введите код";
    private Coroutine _codeEnter;

    private void OnEnable()
    {
        CodeInputScreen.EnterCodeClicked += OnCodeEnter;
    }

    private void OnDisable()
    {
        CodeInputScreen.EnterCodeClicked -= OnCodeEnter;
    }

    private async void OnCodeEnter(string code)
    {
        var task = await CodeInput(code);
        
        if (task != null && task.IsSuccess)
        {
            CodeEnteredSuccess?.Invoke();
        }
    }

    private async Task<TaskResult<bool>> CodeInput(string code)
    {
        OperationEvent.OnMethodCalled();

        BonusCode bonusCode = null;

        if (_fireBaseController.BonusCodeList != null && _fireBaseController.BonusCodeList.Count > 0)
        {
            bonusCode = _fireBaseController.BonusCodeList.FirstOrDefault(bc => bc.Symbol == code.Trim());
        }
        else
        {
            var task = await _fireBaseController.SyncBonusCodes();
            if (task.IsSuccess)
            {
                bonusCode = _fireBaseController.BonusCodeList.FirstOrDefault(bc => bc.Symbol == code.Trim());    
            }
        }
        
        if (string.IsNullOrEmpty(code) || string.Equals(code, codeHint))
        {
            OperationEvent.OnMethodFinished(new TaskResult<bool>("Код не может быть пустым"));
            return new TaskResult<bool>("Код не может быть пустым");
        }

        if (_skinCodeList.Contains(code.Trim()))
        {
            var task = await SaveUserSkins(code);
            if (task.IsSuccess)
            {
                return new TaskResult<bool>(true);
            }

            if (task.IsFailure)
            {
                return new TaskResult<bool>(task.ErrorMessage);
            }
        }
        else if (bonusCode != null)
        {
            var task = await ActivateBonusCode(bonusCode);
            if(task.IsSuccess)
                    return new TaskResult<bool>(true);
            if(task.IsFailure)
                return new TaskResult<bool>(task.ErrorMessage);    
        }
        else
        {
            OperationEvent.OnMethodFinished(new TaskResult<bool>(errorMessage: "Такого кода не существует"));
            return new TaskResult<bool>("Такого кода не существует");
        }

        return null;
    }


    private async Task<TaskResult<bool>> SaveUserSkins(string code)
    {
        try
        {
            switch (code)
            {
                case "A3SK1N":
                    PlayerPrefs.SetInt("A3_SKIN", 1);
                    break;
                case "SUP3RSK1N":
                    PlayerPrefs.SetInt("SUPER_SKIN", 1);
                    break;
                case "M0US3SKIN":
                    PlayerPrefs.SetInt("MOUSE_SKIN", 1);
                    break;
                case "F0XSKIN":
                    PlayerPrefs.SetInt("FOX_SKIN", 1);
                    break;
                case "RA1NB0WSK1N":
                    PlayerPrefs.SetInt("RAINBOW_SKIN", 1); 
                    break;
                case "BUS1NESS_SK1N":
                    PlayerPrefs.SetInt("BUSINESS_SKIN", 1);
                    break;
            }

            //_capyController.DetermineOwnedSkins();
            
            var ownedSkinList = _capyController.OwnedSkins.Where(skin => skin.skinName != "DEFAULT_SKIN").Select(skin => skin.skinName).ToList();
            
            var saveTask = await _fireBaseController.SaveUserSkins(ownedSkinList);
            
            if (saveTask.IsSuccess)
            {
                OperationEvent.OnMethodFinished(new TaskResult<bool>(true));
                return new TaskResult<bool>(true);
            }

            if (saveTask.IsFailure)
            {
                OperationEvent.OnMethodFinished(new TaskResult<bool>(saveTask.ErrorMessage));
                return new TaskResult<bool>(saveTask.ErrorMessage);
            }
        }
        catch (Exception e)
        {
            OperationEvent.OnMethodFinished(new TaskResult<bool>(e.Message));
            return new TaskResult<bool>(e.Message);
        }

        return null;
    }

    private async Task<TaskResult<bool>> ActivateBonusCode(BonusCode code)
    {
        var task = await _fireBaseController.ActivateBonusCode(code);
    
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

        return null;
    }
}
