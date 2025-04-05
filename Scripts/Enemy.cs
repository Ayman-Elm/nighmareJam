using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int CoinDrop = 0;
    public float heatlth = 0f; // Health of the enemy
    public float damage = 0f; // Damage of the enemy
    public float speed = 0f; // Speed of the enemy;
    public float spawnPropability = 0f; // Probability of spawning this enemy

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            Player player = collision.collider.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }
        }
    }
}
