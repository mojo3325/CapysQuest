using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

private class Level
{
    public int
}


[System.Serializable]
private class LevelProgressData
{
    public List<int> completedLevels = new List<int>();
}


public class LevelMenuScreen : MenuScreen
{

    private VisualElement root;

    private Button _backButton;

    private Button _level_1;
    private Button _level_2;
    private Button _level_3;
    private Button _level_4;
    private Button _level_5;
    private Button _level_6;
    private Button _level_7;
    private Button _level_8;
    private Button _level_9;
    private Button _level_10;

    private bool _isLevel1;
    private bool _isLevel2;
    private bool _isLevel3;
    private bool _isLevel4;
    private bool _isLevel5;
    private bool _isLevel6;
    private bool _isLevel7;
    private bool _isLevel8;
    private bool _isLevel9;
    private bool _isLevel10;

    private LevelProgressData progressData = new LevelProgressData();

    protected override void RegisterButtonCallbacks()
    {
        base.RegisterButtonCallbacks();
        base.RegisterButtonCallbacks();

        _backButton.clicked += () =>
        {
            ButtonEvent.OnCloseMenuCalled();
            _mainMenuUIManager.HideLevelMenuScreen();
        };

    }

    protected override void SetVisualElements()
    {
        base.SetVisualElements();
        _showMenuBar = false;

        root = _root.Query<VisualElement>("LevelMenuScreen");
        _backButton = root.Query<Button>("BackButton");
        _level_1 = root.Query<Button>("Level_item_1");
        _level_2 = root.Query<Button>("Level_item_2");
        _level_3 = root.Query<Button>("Level_item_3");
        _level_4 = root.Query<Button>("Level_item_4");
        _level_5 = root.Query<Button>("Level_item_5");
        _level_6 = root.Query<Button>("Level_item_6");
        _level_7 = root.Query<Button>("Level_item_7");
        _level_8 = root.Query<Button>("Level_item_8");
        _level_9 = root.Query<Button>("Level_item_9");
        _level_10 = root.Query<Button>("Level_item_10");
    }


    private void SaveProgress()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/levelProgress.dat";
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, progressData);
        stream.Close();
    }

    private void LoadProgress()
    {
        string path = Application.persistentDataPath + "/levelProgress.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            progressData = formatter.Deserialize(stream) as LevelProgressData;
            stream.Close();
        }
    }
}
