using Assets.SimpleLocalization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FinishScreenController : MonoBehaviour
{
    public static event Action<string> IsReady;

    private string _generatedCode;


    private void OnEnable()
    {
        FinishScreen.IsShown += SetupFinishInfo;
        CapyCharacter.OnCodeGenerated += SetupGeneratedCode;
    }

    private void OnDisable()
    {
        FinishScreen.IsShown -= SetupFinishInfo;
        CapyCharacter.OnCodeGenerated -= SetupGeneratedCode;
    }

    private void SetupFinishInfo()
    {
        LocalizationManager.Read();

        var languagePref = PlayerPrefs.GetString("game_language", "");

        if (languagePref != "")
        {
            LocalizationManager.Language = languagePref;
            var textStart = LocalizationManager.Localize("finish_label_start");
            var textEnd = LocalizationManager.Localize("finish_label_end");
            IsReady?.Invoke(textStart += _generatedCode += textEnd);
        }
    }

    private void SetupGeneratedCode(string code)
    {
        _generatedCode += code;
    }
}
