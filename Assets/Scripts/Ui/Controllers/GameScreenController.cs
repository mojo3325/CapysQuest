using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScreenController : MonoBehaviour
{
    [Header("Player & FinishLineReferences")]
    [SerializeField] public Transform playerTransform;
    [SerializeField] public Transform spawnPointTransform;
    [SerializeField] public Transform finishLineTransform;

    private float _fullDistance;
    private Vector3 _finishLinePosition;
    private Coroutine _progressCoroutine;
    
    private float _currentProgress = 0f;

    public float CurrentLevelProgress => _currentProgress;

    private void OnEnable()
    {
        MenuBar.PlayButtonClicked += (it) => OnPlayClick();
        CapyCharacter.OnCapyDied += (d, v) => OnCapyDie();
    }

    private void OnDisable()
    {
        MenuBar.PlayButtonClicked -= (it) => OnPlayClick();
        CapyCharacter.OnCapyDied -= (d, v) => OnCapyDie();
    }

    private void OnPlayClick()
    {
        _currentProgress = 0f;
        
        if(_progressCoroutine != null)
            StopCoroutine(_progressCoroutine);
        
        _progressCoroutine = StartCoroutine(ProgressCounter());
    }

    private void OnCapyDie()
    {
        if(_progressCoroutine != null)
            StopCoroutine(_progressCoroutine);
    }

    private IEnumerator ProgressCounter()
    {
        while (true)
        {
            float distanceToStart = Vector3.Distance(playerTransform.transform.position, spawnPointTransform.position);
            float totalDistance = Vector3.Distance(spawnPointTransform.position, finishLineTransform.position);
            _currentProgress = Mathf.Clamp01(distanceToStart / totalDistance);
            yield return null;
        }
    }
}
