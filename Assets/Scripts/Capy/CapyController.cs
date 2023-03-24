using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class CapyController : MonoBehaviour
{
    public static event Action CapyDiedThreeTimes;

    [SerializeField] private GameObject bloodPrefab;
    [SerializeField] private GameObject waterPrefab;
    private int CapyDieCount;

    private void OnEnable()
    {
        CapyCharacter.OnCapyDied += ParticleSpawn;
        CapyCharacter.OnCapyDied += CountCapyDies;
        IntAdController.OnAdSuccessClosed += ResetDieCount;
    }

    private void OnDisable()
    {
        CapyCharacter.OnCapyDied -= ParticleSpawn;
        CapyCharacter.OnCapyDied -= CountCapyDies;
        IntAdController.OnAdSuccessClosed -= ResetDieCount;
    }

    private void CountCapyDies(DieType D, Vector3 V)
    {
        Debug.Log("CapyDieCount Called");

        if(CapyDieCount >= 2)
        {
            CapyDiedThreeTimes?.Invoke();
        }
        else
        {
            CapyDieCount += 1;
        }

        Debug.Log("Capy Die Count : " + CapyDieCount);

    }

    private void ResetDieCount()
    {
        CapyDieCount = 0;
    }

    private void ParticleSpawn(DieType dieType, Vector3 targetPosition)
    {
        for (int i = 0; i < 15; i++)
        {
            Vector3 direction = new Vector3(Random.Range(-1f, 1f), Random.Range(0f, 1f), Random.Range(-1f, 1f)).normalized * 80f;
            Vector3 position = new Vector3(targetPosition.x, targetPosition.y + 5, 0);
            GameObject particle = Instantiate((dieType == DieType.Enemy || dieType == DieType.Timer) ? bloodPrefab : waterPrefab, position, Quaternion.identity);
            particle.GetComponent<Rigidbody2D>().AddForce(direction, ForceMode2D.Impulse);
        }
    }
}
