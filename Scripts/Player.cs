using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    public float speed = 5.0f;
    public float health = 5.0f;
    public float energy = 100f;

    public float energyDrainRate = 10f;
    public float energyRechargeRate = 15f;
    public float rechargeDelay = 3f;

    private float lastFlashlightUseTime;
    private LightMod flashlight;

    public float invincibilityDuration = 0.5f;
    private bool isInvincible = false;

void Start()
{
    flashlight = GetComponentInChildren<LightMod>();
    lastFlashlightUseTime = Time.time;

    ApplyAmplifiers(); // ✅ This replaces the original amplifier code
}

    void Update()
    {
        HandleFlashlightEnergy();
       // Debug.Log("Energy: " + energy.ToString("F1"));
    }
    public void ApplyAmplifiers()
{
    if (GameManager.Instance != null)
    {
        speed = 5.0f * GameManager.Instance.speedAmplifier;
        health = 5.0f * GameManager.Instance.healthAmplifier;
        energy = 100f * GameManager.Instance.energyAmplifier;
    }

    if (flashlight != null)
    {
        flashlight.ApplyAmplifiersFromGameManager();
    }
}


    void HandleFlashlightEnergy()
    {
        bool isUsingFlashlight = flashlight.GetIsFlashlightOn();

        if (isUsingFlashlight && energy > 0f)
        {
            energy -= energyDrainRate * Time.deltaTime;
            lastFlashlightUseTime = Time.time;

            if (energy <= 0f)
            {
                energy = 0f;
                flashlight.ForceDisable();
            }
        }

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
