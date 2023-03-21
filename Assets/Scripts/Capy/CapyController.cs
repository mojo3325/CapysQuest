using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapyController : MonoBehaviour
{

    [SerializeField] private GameObject bloodPrefab;
    [SerializeField] private GameObject waterPrefab;

    private void OnEnable()
    {
        CapyCharacter.OnCapyDied += ParticleSpawn;
    }

    private void OnDisable()
    {
        CapyCharacter.OnCapyDied -= ParticleSpawn;
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
