using UnityEngine;

public class Player : MonoBehaviour
{

    public float speed = 5.0f; // Speed of the player
    public float health = 5.0f; // Health of the player
    public float energy = 0f; // Energy of the player
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        Debug.Log("Player took " + amount + " damage! Current HP: " + health);

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player died!");
        // TODO: Add death logic (disable movement, reload scene, etc.)
        Destroy(gameObject); // simple placeholder for now
    }
}
