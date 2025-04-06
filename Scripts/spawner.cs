using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform player;
    public float spawnRadius = 20f;
    public float minAngleFromView = 30f; // degrees away from camera/player forward
    public float spawnInterval = 5f;     // changeable in Inspector

    private float timer;

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
        Vector2 forward2D = transform.up; // or whatever is "forward" in your 2D world
        Vector3 spawnPosition = Vector3.zero;

        int attempts = 0;
        while (attempts < 100)
        {
            // 1) Get a random direction in 2D
            Vector2 randomDir2D = Random.insideUnitCircle.normalized;

            // 2) Check angle (in 2D) relative to our "forward2D"
            float angle = Vector2.Angle(forward2D, randomDir2D);

            if (angle > minAngleFromView)
            {
                // 3) Generate final spawn position in XY plane
                spawnPosition = (Vector2)player.position 
                                + randomDir2D * spawnRadius;
                spawnPosition.z = 0f; // ensure not behind background
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
            Debug.LogWarning("Failed to find a valid 2D spawn position after 100 attempts.");
        }
    }
}
