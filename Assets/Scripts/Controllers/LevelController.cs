using Assets.Scripts.Ui.Screens;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
/*
    [Header("RandomSaws")]
    [SerializeField] private GameObject[] randomSaws;*/

    [Header("World Objects")]
    [SerializeField] private GameObject Sun;

    private BoosterController[] boosters;

    private void Start()
    {
        boosters = FindObjectsOfType<BoosterController>();
    }

    private void OnEnable()
    {
        /*        MenuBar.PlayButtonClicked += StartRandomChoose;
        */
        MenuBar.PlayButtonClicked += (level) => OnPlayClicked(level);
    }

    private void OnDisable()
    {
        /*        MenuBar.PlayButtonClicked -= StartRandomChoose;
        */
        MenuBar.PlayButtonClicked -= (level) => OnPlayClicked(level);
    }

    private void ResetAllBoosters()
    {
        foreach(BoosterController bc in boosters)
        {
            bc.ResetState();
        }
    }

    private void OnPlayClicked(Level level)
    {
        ResetAllBoosters();

        if (level >= Level.LEVEL6 && level <= Level.LEVEL10)
            SunTurnOff();
        else if(level >= Level.LEVEL1 && level <= Level.LEVEL5)
            SunTurnOn();
    }
    
    private void SunTurnOff()
    {
        Sun.gameObject.SetActive(false);
    }

    private void SunTurnOn()
    {
        Sun.gameObject.SetActive(true);
    }
    
/*    private void StartRandomChoose()
    {
        StartCoroutine(RandomSawChoose());
    }
*/
/*    private IEnumerator RandomSawChoose()
    {
        yield return null;
        List<int> randomIndices = new List<int>();
        for (int i = 0; i < 10; i++)
        {
            int randomIndex = Random.Range(0, 15);
            while (randomIndices.Contains(randomIndex))
            {
                randomIndex = Random.Range(0, 15);
            }
            randomIndices.Add(randomIndex);
        }

        foreach (int randomIndex in randomIndices)
        {
            randomSaws[randomIndex].GetComponent<RandomSawController>().SeActive();
        }
    }*/
}
