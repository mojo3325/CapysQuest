using Firebase.Analytics;
using System.Buffers.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Ui.Screens;
using Unity.VisualScripting;

public class LevelMenuController : MonoBehaviour
{
    [Header("Levels SpawnPoint")]
    [SerializeField] Transform level1_spawn;
    [SerializeField] Transform level1_finish;
    [SerializeField] Transform level2_spawn;
    [SerializeField] Transform level2_finish;
    [SerializeField] Transform level3_spawn;
    [SerializeField] Transform level3_finish;
    [SerializeField] Transform level4_spawn;
    [SerializeField] Transform level4_finish;
    [SerializeField] Transform level5_spawn;
    [SerializeField] Transform level5_finish;
    [SerializeField] Transform level6_spawn;
    [SerializeField] Transform level6_finish;
    [SerializeField] Transform level7_spawn;
    [SerializeField] Transform level7_finish;
    [SerializeField] Transform level8_spawn;
    [SerializeField] Transform level8_finish;
    [SerializeField] Transform level9_spawn;
    [SerializeField] Transform level9_finish;
    [SerializeField] Transform level10_spawn;
    [SerializeField] Transform level10_finish;

    [Header("GameScreenController")]
    [SerializeField] GameScreenController _gameScreenController;

    [Header("Main Character")]
    [SerializeField] GameObject mainCharacter;

    [Header("MenuController")]
    [SerializeField] MainMenuUIManager _mainMenuManager;


    private Transform characterSpawnpoint;

    public Level currentLevel = Level.LEVEL1;

    private void OnEnable()
    {
        LevelMenuScreen.OnLevelPlayClick += OnLevelPlayClick;
        MenuBar.PlayButtonClicked += (level) => OnPlayClick();
        CapyCharacter.OnCapyDied += OnCapyDie;
    }

    private void OnDisable()
    {
        LevelMenuScreen.OnLevelPlayClick -= OnLevelPlayClick;
        MenuBar.PlayButtonClicked -= (level) => OnPlayClick();
        CapyCharacter.OnCapyDied -= OnCapyDie;
    }

    private void OnLevelPlayClick(Level level)
    {
        switch (level)
        {
            case Level.LEVEL1:
                characterSpawnpoint = level1_spawn;
                _gameScreenController.finishLineTransform = level1_finish;
                _gameScreenController.spawnPointTransform = level1_spawn;
                break;
            case Level.LEVEL2:
                characterSpawnpoint = level2_spawn;
                _gameScreenController.finishLineTransform = level2_finish;
                _gameScreenController.spawnPointTransform = level2_spawn;
                break;
            case Level.LEVEL3:
                characterSpawnpoint = level3_spawn;
                _gameScreenController.finishLineTransform = level3_finish;
                _gameScreenController.spawnPointTransform = level3_spawn;
                break;
            case Level.LEVEL4:
                characterSpawnpoint = level4_spawn;
                _gameScreenController.finishLineTransform = level4_finish;
                _gameScreenController.spawnPointTransform = level4_spawn;
                break;
            case Level.LEVEL5:
                characterSpawnpoint = level5_spawn;
                _gameScreenController.finishLineTransform = level5_finish;
                _gameScreenController.spawnPointTransform = level5_spawn;
                break;
            case Level.LEVEL6:
                characterSpawnpoint = level6_spawn;
                _gameScreenController.finishLineTransform = level6_finish;
                _gameScreenController.spawnPointTransform = level6_spawn;
                break;
            case Level.LEVEL7:
                characterSpawnpoint = level7_spawn;
                _gameScreenController.finishLineTransform = level7_finish;
                _gameScreenController.spawnPointTransform = level7_spawn;
                break;
            case Level.LEVEL8:
                characterSpawnpoint = level8_spawn;
                _gameScreenController.finishLineTransform = level8_finish;
                _gameScreenController.spawnPointTransform = level8_spawn;
                break;
            case Level.LEVEL9:
                characterSpawnpoint = level9_spawn;
                _gameScreenController.finishLineTransform = level9_finish;
                _gameScreenController.spawnPointTransform = level9_spawn;
                break;
            case Level.LEVEL10:
                characterSpawnpoint = level10_spawn;
                _gameScreenController.finishLineTransform = level10_finish;
                _gameScreenController.spawnPointTransform = level10_spawn;
                break;
            default:
                break;
        }

        currentLevel = level;
        CapyToSpawn();
        _mainMenuManager.ShowGameScreen();
    }


    private void OnPlayClick()
    {
        if (mainCharacter.transform.localPosition != characterSpawnpoint.localPosition)
        {
            mainCharacter.SetActive(false);
            CapyToSpawn();
        }
    }

    private void OnCapyDie(DieType dieType, Vector3 position)
    {
        CapyToSpawn();
    }

    private void CapyToSpawn()
    {
        mainCharacter.transform.position = characterSpawnpoint.position;
        mainCharacter.transform.localRotation = Quaternion.identity;
        if (mainCharacter.transform.localScale.x != 2)
        {
            mainCharacter.transform.localScale = new Vector3(2, mainCharacter.transform.localScale.y, mainCharacter.transform.localScale.z);
        }
    }
}

