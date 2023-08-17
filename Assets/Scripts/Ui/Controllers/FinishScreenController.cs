using System;
using System.Collections.Generic;
using UnityEngine;

public class FinishScreenController : MonoBehaviour
{
    public static event Action<string> IsReady;
    private Dictionary<string, object> _gameStrings;

    private void OnEnable()
    {
        CapyCharacter.OnFinishAchieved += SetupFinishInfo;
        GameStringsController.GameStringsInitialized += GameStringsInit;
    }

    private void GameStringsInit(Dictionary<string, object> strings)
    {
        _gameStrings = strings;
    }

    private void OnDisable()
    {
        CapyCharacter.OnFinishAchieved -= SetupFinishInfo;
        GameStringsController.GameStringsInitialized -= GameStringsInit;
    }

    private void SetupFinishInfo(Level level)
    {
        if (_gameStrings != null)
        {
            if (level < Level.LEVEL10)
            {
                var first = _gameStrings["winner_first_text"];
                var second = _gameStrings[level.ToString()];
                IsReady?.Invoke(first + " " + second);
            }
            else if(level == Level.LEVEL10)
            {
                var textEnd = _gameStrings["winnner_second_text"];
                IsReady?.Invoke(textEnd.ToString());
            }
        }
    }
}
