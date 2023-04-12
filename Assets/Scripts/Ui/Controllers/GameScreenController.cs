using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScreenController : MonoBehaviour
{
    [Header("Player & FinishLineReferences")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform finishLineTransform;

    private float _fullDistance;
    private Vector3 _finishLinePosition;
    private Coroutine _progressCoroutine;
    
    private float _currentProgress = 0f;

    public float CurrentLevelProgress => _currentProgress;

    private float GetDistance()
    {
        return Vector3.Distance(playerTransform.position, finishLineTransform.position);
    }
    private void OnEnable()
    {
        MenuBar.PlayButtonClicked += OnPlayClick;
        CapyCharacter.OnCapyDied += (d, v) => OnCapyDie();
    }

    private void OnDisable()
    {
        MenuBar.PlayButtonClicked -= OnPlayClick;
        CapyCharacter.OnCapyDied -= (d, v) => OnCapyDie();
    }

    private void Start()
    {
        _finishLinePosition = finishLineTransform.position;
        _fullDistance = GetDistance();
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
            var currentDistance = GetDistance();
            _currentProgress = Mathf.InverseLerp( _fullDistance, 0f, currentDistance);
            yield return null;
        }
    }
}
