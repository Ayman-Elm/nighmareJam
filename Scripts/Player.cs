using UnityEngine;
using System.Collections;
using FMODUnity;
using FMOD.Studio;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [Header("SCENE")]
    public string returnSceneName = "GameScene";

    public float speed = 5.0f;
    public float health = 5.0f;
    public float energy = 100f;

    [Header("FMOD Events")]
    [SerializeField] public EventReference playerDamagedSound;
    [SerializeField] public EventReference playerDeathSound;

    public float energyDrainRate = 10f;
    public float energyRechargeRate = 15f;
    public float rechargeDelay = 3f;

    private float lastFlashlightUseTime;
    private LightMod flashlight;
    public EventInstance batteryLifeInstance;

    public float invincibilityDuration = 0.5f;
    private bool isInvincible = false;

    void Start()
    {
        flashlight = GetComponentInChildren<LightMod>();
        lastFlashlightUseTime = Time.time;

        // Create the battery life event instance but don't start it
        batteryLifeInstance = RuntimeManager.CreateInstance("event:/FlashLightOn");

        ApplyAmplifiers();
    }

    void OnDestroy()
    {
        if (batteryLifeInstance.isValid())
        {
            batteryLifeInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            batteryLifeInstance.release();
        }
    }

    void Update()
    {
        HandleFlashlightEnergy();
        UpdateBatteryLifeParameter();
    }

    void UpdateBatteryLifeParameter()
    {
        if (batteryLifeInstance.isValid() && flashlight.GetIsFlashlightOn())
        {
            // Set the Battery Life parameter based on current energy (1-100)
            batteryLifeInstance.setParameterByName("Battery Life", energy);
        }
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

        // Play damage sound
        if (!playerDamagedSound.IsNull)
        {
            AudioManager.Instance.PlayOneShot(playerDamagedSound, transform.position);
        }

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
        // Play death sound
        if (!playerDeathSound.IsNull)
        {
            AudioManager.Instance.PlayOneShot(playerDeathSound, transform.position);
        }
        
        // Stop all instances of the MainMusic event
        RuntimeManager.StudioSystem.getEvent("event:/MainMusic", out var eventDescription);
        if (eventDescription.isValid())
        {
            eventDescription.getInstanceList(out var instances);
            foreach (var instance in instances)
            {
                instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                instance.release();
            }
        }
        
        Destroy(gameObject);
        SceneManager.LoadScene(returnSceneName);
    }

    private IEnumerator InvincibilityFrames()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibilityDuration);
        isInvincible = false;
    }
}
