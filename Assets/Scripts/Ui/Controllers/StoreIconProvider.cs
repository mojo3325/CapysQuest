//
// using System;
// using System.Collections.Generic;
// using Unity.Profiling;
// using UnityEngine;
// using UnityEngine.Purchasing;
//
// public class StoreIconProvider
// {
//     public static Dictionary<string, Sprite> Icons
//     {
//         get;
//         private set;
//
//     } = new();
//
//     private static int TargetIconCount;
//     public static event Action OnLoadComplete;
//
//     public static void Initialize(ProductCollection products)
//     {
//         if (Icons.Count == 0)
//         {
//             TargetIconCount = products.all.Length;
//             foreach (var product in products.all)
//             {
//                 var operation = Resources.LoadAsync<Sprite>()
//             }
//         }
//         else
//         {
//             Debug.Log("StoreIcon provider init error");
//         }
//     } 
// }
