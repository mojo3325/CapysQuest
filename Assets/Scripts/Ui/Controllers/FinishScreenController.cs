using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Google.MiniJSON;
using UnityEngine;

public class FinishScreenController : MonoBehaviour
{
    public static event Action<string> IsReady;
    private string _generatedCode;
    private Dictionary<string, object> _gameStrings;

    private void OnEnable()
    {
        FinishScreen.IsShown += SetupFinishInfo;
        CapyCharacter.OnCodeGenerated += SetupGeneratedCode;
        MenuManagerController.GameStringsInitialized += GameStringsInit;
    }

    private void GameStringsInit(Dictionary<string, object> strings)
    {
        _gameStrings = strings;
    }

    private void OnDisable()
    {
        FinishScreen.IsShown -= SetupFinishInfo;
        CapyCharacter.OnCodeGenerated -= SetupGeneratedCode;
        MenuManagerController.GameStringsInitialized -= GameStringsInit;
    }

    private void SetupFinishInfo()
    {
        //
        // if (gameStrings != null)
        // {
        //     var textStart = gameStrings["winner_first_text"];
        //     var textEnd = gameStrings["winnner_second_text"];
        //     IsReady?.Invoke(textStart + " " + _generatedCode + " " + textEnd);
        // }
    }

    private void SetupGeneratedCode(string code)
    {
        _generatedCode += code;
    }
}
