using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{

    [Header("RandomSaws")]
    [SerializeField] private GameObject[] randomSaws;

    [Header("World Objects")]
    [SerializeField] private GameObject Sun;

    private BoosterController[] boosters;

    private void Start()
    {
        boosters = FindObjectsOfType<BoosterController>();
    }

    private void OnEnable()
    {
        MenuBar.PlayButtonClicked += StartRandomChoose;
        ZoneController.OnZoneAchieved += OnZone3Achieved;
        MenuBar.PlayButtonClicked += SunTurnOn;
        MenuBar.PlayButtonClicked += ResetAllBoosters;
    }

    private void OnDisable()
    {
        MenuBar.PlayButtonClicked -= StartRandomChoose;
        ZoneController.OnZoneAchieved -= OnZone3Achieved;
        MenuBar.PlayButtonClicked -= SunTurnOn;
        MenuBar.PlayButtonClicked -= ResetAllBoosters;
    }

    private void ResetAllBoosters()
    {
        foreach(BoosterController bc in boosters)
        {
            bc.ResetState();
        }
    }

    private void OnZone3Achieved(ZoneType zoneType)
    {
        if(zoneType == ZoneType.zone_3)
            SunTurnOff();
    }
    
    private void SunTurnOff()
    {
        Sun.gameObject.SetActive(false);
    }

    private void SunTurnOn()
    {
        Sun.gameObject.SetActive(true);
    }
    
    private void StartRandomChoose()
    {
        StartCoroutine(RandomSawChoose());
    }

    private IEnumerator RandomSawChoose()
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
    }
}
