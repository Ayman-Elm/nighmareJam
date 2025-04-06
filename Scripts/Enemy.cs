using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int baseCoinDrop = 0;
    public float baseHealth = 0f;
    public float baseDamage = 0f;
    public float baseSpeed = 0f;
    public float baseSpawnPropability = 0f;

    private int coinDrop = 0;
    private float health = 0f;
    private float damage = 0f;
    private float speed = 0f;
    private float spawnPropability = 0f;

    void Start()
    {
        int level = GameManager.Instance != null ? GameManager.Instance.level : 1;

        // 📈 Scale factors
        float healthMultiplier = 1f + (level - 1) * 0.3f;
        float damageMultiplier = 1f + (level - 1) * 0.2f;
        float speedMultiplier = 1f + (level - 1) * 0.1f;
        float coinMultiplier = 1f + (level - 1) * 0.15f;

        // ✅ Apply scaling
        health = baseHealth * healthMultiplier;
        damage = baseDamage * damageMultiplier;
        speed = baseSpeed * speedMultiplier;
        coinDrop = Mathf.CeilToInt(baseCoinDrop * coinMultiplier);
        spawnPropability = baseSpawnPropability;
    }

    

    void Update()
    {
        // Example if you want to use speed to move the enemy
        // transform.Translate(Vector2.left * speed * Time.deltaTime);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }
        }
    }

    // ✅ Public access for other scripts
    public void TakeDamage(float amount)
    {
        health -= amount;
    }

    public float GetHealth() => health;
    public float GetSpeed() => speed;
    public int GetCoinDrop() => coinDrop;
}
