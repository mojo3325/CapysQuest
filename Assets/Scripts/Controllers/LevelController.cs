using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{

    [Header("RandomSaws")]
    [SerializeField] private GameObject[] randomSaws;


    [Header("World Objects")]
    [SerializeField] private GameObject Sun;

    private void OnEnable()
    {
        MenuBar.PlayButtonClicked += StartRandomChoose;
        CapyCharacter.OnZoneAchieved += SunTurnOff;
        MenuBar.PlayButtonClicked += SunTurnOn;
    }

    private void OnDisable()
    {
        MenuBar.PlayButtonClicked -= StartRandomChoose;
        CapyCharacter.OnZoneAchieved -= SunTurnOff;
        MenuBar.PlayButtonClicked -= SunTurnOn;
    }

    private void SunTurnOff(ZoneType zoneType)
    {
        if(zoneType == ZoneType.zone_3)
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
        for (int i = 0; i < 7; i++)
        {
            int randomIndex = Random.Range(0, 16);
            while (randomIndices.Contains(randomIndex))
            {
                randomIndex = Random.Range(0, 16);
            }
            randomIndices.Add(randomIndex);
        }

        foreach (int randomIndex in randomIndices)
        {
            randomSaws[randomIndex].GetComponent<RandomSawController>().SeActive();
        }
    }
}
