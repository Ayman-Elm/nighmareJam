using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{

    public float speed = 5.0f; // Speed of the player
    public float health = 5.0f; // Health of the player
    public float energy = 100f; // Energy of the player
    public float energyDrainRate = 10f;       // Energy per second while flashlight is on
    public float energyRechargeRate = 15f;    // Energy per second when recharging
    public float rechargeDelay = 3f;          // Time in seconds before recharge starts

    private float lastFlashlightUseTime; // Time when the flashlight was last used
    private bool isRecharging = false; // Is the player recharging energy?

    private LightMod flashlight; // Reference to the LightMod script for flashlight control
    public float invincibilityDuration = 0.5f; // Duration of invincibility after taking damage
    private bool isInvincible = false; // Is the player invincible?

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        flashlight = GetComponentInChildren<LightMod>(); // Assumes flashlight is a child GameObject
        lastFlashlightUseTime = Time.time; // Initialize the last use time
    }

    // Update is called once per frame
    void Update()
    {
        HandleFlashlightEnergy();
       // Debug.Log("Energy: " + energy.ToString("F1"));
    }
    void HandleFlashlightEnergy()
    {
        // Check if flashlight is enabled
        bool isUsingFlashlight = flashlight.GetIsFlashlightOn();

        if (isUsingFlashlight && energy > 0)
        {
            energy -= energyDrainRate * Time.deltaTime;
            lastFlashlightUseTime = Time.time;

            if (energy <= 0)
            {
                energy = 0;
                flashlight.ForceDisable(); // turn off the light
            }
        }

        // Start recharging after delay
        if (!isUsingFlashlight && Time.time - lastFlashlightUseTime > rechargeDelay)
        {
            if (energy < 100f)
            {
                energy += energyRechargeRate * Time.deltaTime;
                if (energy > 100f) energy = 100f;
            }
        }
    }

    public void TakeDamage(float amount)
    {
        if (isInvincible) return;

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
        yield return new WaitForSeconds(invincibilityDuration);
        isInvincible = false;
    }
}
