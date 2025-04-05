using UnityEngine;

public class spawner : MonoBehaviour
{
public GameObject enemyPrefab;
    public Transform player;
    public float spawnRadius = 20f;
    public float minAngleFromView = 30f; // degrees away from camera forward
    public float spawnInterval = 5f;     // ⏱️ changeable in Inspector

    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnEnemyOutOfView();
            timer = 0f;
        }
    }

    void SpawnEnemyOutOfView()
    {
        Vector3 spawnPosition = Vector3.zero;
        int attempts = 0;

        while (attempts < 100)
        {
            Vector3 randomDirection = Random.onUnitSphere;
            randomDirection.y = 0; // horizontal plane only

            float angle = Vector3.Angle(transform.forward, randomDirection);

            if (angle > minAngleFromView)
            {
                spawnPosition = player.position + randomDirection.normalized * spawnRadius;
                break;
            }

            attempts++;
        }

        if (spawnPosition != Vector3.zero)
        {
            Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("Failed to find valid spawn position after 100 attempts.");
        }
    }
}