using System;
using System.Collections.Generic;
using Google.MiniJSON;
using UnityEngine;

public class GameStringsController : MonoBehaviour
{
    public static event Action<Dictionary<string, object>> GameStringsInitialized;
    
    private void Start()
    {
        var myStringsFile = Resources.Load("GameStrings");
        var gameStrings = Json.Deserialize(myStringsFile.ToString()) as Dictionary<string, object>;
        
        if (gameStrings != null && gameStrings.Count != 0)
        {
            GameStringsInitialized?.Invoke(gameStrings);
        }
    }
}
