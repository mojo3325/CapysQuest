using System;
using UnityEngine;
using UnityEngine.UIElements;

public class LoadingScreen : MenuScreen
{
    private VisualElement _loadCat;

    private static string _loadCatName = "LoadCat";
    
    protected override void SetVisualElements()
    {
        base.SetVisualElements();

        _loadCat = _root.Q<VisualElement>(_loadCatName); 
    }

    private void Update()
    {
        _loadCat.transform.rotation = Quaternion.Euler(0f, 0f, _loadCat.transform.rotation.eulerAngles.z + 150f * Time.deltaTime);
    }
}
