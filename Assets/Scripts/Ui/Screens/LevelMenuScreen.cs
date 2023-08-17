using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scripts.Ui.Screens
{
    [System.Serializable]
    public class LevelProgress
    {
        public List<LevelClass> value = new List<LevelClass>
        {
        new LevelClass(1, true),
        new LevelClass(2, false),
        new LevelClass(3, false),
        new LevelClass(4, false),
        new LevelClass(5, false),
        new LevelClass(6, false),
        new LevelClass(7, false),
        new LevelClass(8, false),
        new LevelClass(9, false),
        new LevelClass(10, false),
        };
    }

    [System.Serializable]
    public class LevelClass
    {
        public int number;
        public bool isCompleted;

        public LevelClass(int number, bool isCompleted)
        {
            this.number = number;
            this.isCompleted = isCompleted;
        }
    }


    public class LevelMenuScreen : MenuScreen
    {

        [SerializeField] string saveFilename = "levelProgress.json";

        public static event Action<Level> OnLevelPlayClick;

        private VisualElement root;

        private Button _backButton;

        [SerializeField] private List<LevelClass> levelsProgress;

        private Level[] levelValues = (Level[])Enum.GetValues(typeof(Level));

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

        public override void ShowScreen()
        {
            base.ShowScreen();
            LoadProgress();
            SetupPlayerLevels();
        }

        private void OnEnable()
        {
            FinishScreen.NextLevelClicked += (currentLevel) =>
            {
                int currentIndex = Array.IndexOf(levelValues, currentLevel);
                if (currentIndex != -1 && currentIndex < levelValues.Length - 1)
                {
                    Level nextLevel = levelValues[currentIndex + 1];
                    OnLevelPlayClick?.Invoke(nextLevel);
                }
            };
            CapyCharacter.OnFinishAchieved += OnLevelAchieved;
        }

        private void OnDisable()
        {
            FinishScreen.NextLevelClicked -= (currentLevel) =>
            {
                int currentIndex = Array.IndexOf(levelValues, currentLevel);
                if (currentIndex != -1 && currentIndex < levelValues.Length - 1)
                {
                    Level nextLevel = levelValues[currentIndex + 1];
                    OnLevelPlayClick?.Invoke(nextLevel);
                }
            };
            CapyCharacter.OnFinishAchieved -= OnLevelAchieved;
        }

        protected override void SetVisualElements()
        {
            base.SetVisualElements();
            _showMenuBar = false;

            root = _root.Query<VisualElement>("LevelMenuScreen");
            _backButton = root.Query<Button>("BackButton");
        }
        public void OnLevelAchieved(Level level)
        {
            int levelNumber;

            switch (level)
            {
                case Level.LEVEL1:
                    levelNumber = 1;
                    break;
                case Level.LEVEL2:
                    levelNumber = 2;
                    break;
                case Level.LEVEL3:
                    levelNumber = 3;
                    break;
                case Level.LEVEL4:
                    levelNumber = 4;
                    break;
                case Level.LEVEL5:
                    levelNumber = 5;
                    break;
                case Level.LEVEL6:
                    levelNumber = 6;
                    break;
                case Level.LEVEL7:
                    levelNumber = 7;
                    break;
                case Level.LEVEL8:
                    levelNumber = 8;
                    break;
                case Level.LEVEL9:
                    levelNumber = 9;
                    break;
                case Level.LEVEL10:
                    levelNumber = 10;
                    break;
                default:
                    Debug.LogWarning("Unhandled level: " + level);
                    return;
            }

            LevelClass levelData = levelsProgress.Find(l => l.number == levelNumber);

            if (levelData != null)
            {
                levelData.isCompleted = true;
                SaveProgress();
            }
        }

        public void SaveProgress()
        {
            string jsonFile = JsonUtility.ToJson(new LevelProgress { value = levelsProgress });

            if (FileManager.WriteToFile(saveFilename, jsonFile))
            {
                Debug.Log("LevelProgressManager.SaveLevelProgress: " + saveFilename + " json string: " + jsonFile);
            }
        }

        public void LoadProgress()
        {
            try
            {
                if (FileManager.LoadFromFile(saveFilename, out var jsonString))
                {
                    Debug.Log("JsonString in load progress == " + jsonString);
                    levelsProgress = JsonUtility.FromJson<LevelProgress>(jsonString).value;
                }
                else
                {
                    Debug.Log("file doesnt exist in load ");
                    levelsProgress = new LevelProgress().value;
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }

        private void SetupPlayerLevels()
        {
            for (int i = 1; i <= 10; i++)
            {

                int levelNumber = i;

                Button levelButton = root.Query<Button>($"Level_item_{i}");
                VisualElement levelAvailability = levelButton.Q<VisualElement>("lock");
                TextElement levelNumberText = levelButton.Q<TextElement>("number");

                LevelClass previusLevelData = levelsProgress.Find(l => l.number == levelNumber-1);

                Level levelEnumValue = (Level)Enum.Parse(typeof(Level), "LEVEL" + levelNumber);

                if (previusLevelData != null)
                {
                    if (previusLevelData.isCompleted)
                    {
                        levelAvailability.style.display = DisplayStyle.None;
                        levelNumberText.style.display = DisplayStyle.Flex;
                    }
                    else
                    {
                        levelAvailability.style.display = DisplayStyle.Flex;
                        levelNumberText.style.display = DisplayStyle.None;
                    }
                }

                levelButton.clicked += () =>
                {
                    ButtonEvent.OnEnterButtonCalled();

                    if (previusLevelData != null && previusLevelData.isCompleted)
                    {
                        OnLevelPlayClick?.Invoke(levelEnumValue);
                    }

                    if(levelNumber == 1)
                        OnLevelPlayClick?.Invoke(levelEnumValue);
                };
            }
        }

    }
}