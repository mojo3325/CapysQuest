using System;
using System.Collections;
using UnityEngine;

public class RandomSawController : MonoBehaviour
{
    private bool _isActive = false;
    private bool _isEndReached;
    private Vector3 startPoint;
    private Vector3 endPoint;
    private Coroutine _moveCoroutine;

    [SerializeField] private Transform _endPointTransform = null;


    void Start()
    {
        Application.targetFrameRate = 60;
        startPoint = transform.localPosition;

        if (_endPointTransform != null)
        {
            endPoint = _endPointTransform.localPosition;
        }
        else
        {
            endPoint = new Vector3(transform.localPosition.x, -42.5f, transform.localPosition.z);
        }
    }

    private void OnEnable()
    {
        MenuBar.PlayButtonClicked += (it) => SetInactive();
    }
    private void OnDisable()
    {
        MenuBar.PlayButtonClicked -= (it) => SetInactive();
    }

    private IEnumerator MoveObject()
    {
        while (_isActive)
        {
            if (!_isEndReached)
            {
                float step = 14f * Time.deltaTime;
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, endPoint, step);
                if (transform.localPosition == endPoint)
                {
                    _isEndReached = true;
                }
            }
            else
            {
                float step = 14f * Time.deltaTime;
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, startPoint, step);
                if (transform.localPosition == startPoint)
                {
                    _isEndReached = false;
                }
            }
            yield return null;
        }
    }

    public void SeActive()
    {
        _isActive = true;
        _moveCoroutine = StartCoroutine(MoveObject());
    }

    public void SetInactive()
    {
        _isActive = false;
        if (_moveCoroutine != null)
            StopCoroutine(_moveCoroutine);
        transform.localPosition = startPoint;
    }
}
