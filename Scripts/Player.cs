using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{

    public float speed = 5.0f; // Speed of the player
    public float health = 5.0f; // Health of the player
    public float energy = 0f; // Energy of the player

    public float invincibilityDuration = 0.5f; // Duration of invincibility after taking damage
    private bool isInvincible = false; // Is the player invincible?

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
        if (isInvincible) return; // Don't take damage if currently invincible

        health -= amount;
        Debug.Log("Player took " + amount + " damage! Current HP: " + health);

        if (health <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(InvincibilityFrames());
        }
    }

    private void Die()
    {
        Debug.Log("Player died!");
        Destroy(gameObject);
    }

    private IEnumerator InvincibilityFrames()
    {
        isInvincible = true;
        // Optional: flash sprite, play SFX, visual feedback
        yield return new WaitForSeconds(invincibilityDuration);
        isInvincible = false;
    }
}
