using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
[RequireComponent(typeof(Collider2D))]
public class LightMod : MonoBehaviour
{
    [Header("Light Settings")]
    public Color lightColor = Color.white;
    [Range(0, 5)] public float intensity = 1f;

    [Header("Spot Settings (for 2D Spot Light)")]
    [Range(0, 360)] public float innerAngle = 70.588f;
    [Range(0, 360)] public float outerAngle = 70.588f;
    [Range(0, 10)] public float innerRadius = 1f;
    [Range(0, 10)] public float outerRadius = 5.59f;
    [Range(0, 1)] public float falloffStrength = 0.109f;

    [Header("Light Toggle")]
    // When true, the light is only enabled while the left mouse button is held down.
    public bool onlyOnLeftMouse = false;

<<<<<<< HEAD
    [Header("Attack Settings")]
    [Tooltip("Damage dealt per hit.")]
    public float damage = 5f;

    [Tooltip("Attacks per second.")]
    public float attackSpeed = 1f;

=======
>>>>>>> 27a0b594de65a68bf776e92678479ee21957b4f7
    private Light2D _light2D;

    // Track when each enemy is next allowed to be hit.
    private Dictionary<Collider2D, float> _nextAttackTime = new Dictionary<Collider2D, float>();

    private void Awake()
    {
        _light2D = GetComponent<Light2D>();

        // Ensure the collider is a trigger for OnTriggerEnter/Stay/Exit
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) 
        {
            col.isTrigger = true;
        }
    }

    private void Update()
    {
        // Update Light2D properties from public fields
        _light2D.color                 = lightColor;
        _light2D.intensity             = intensity;
        _light2D.pointLightInnerAngle  = innerAngle;
        _light2D.pointLightOuterAngle  = outerAngle;
        _light2D.pointLightInnerRadius = innerRadius;
        _light2D.pointLightOuterRadius = outerRadius;
        _light2D.falloffIntensity      = falloffStrength;
        
        // Toggle light based on left mouse button if onlyOnLeftMouse is true
        if (onlyOnLeftMouse)
        {
            _light2D.enabled = Input.GetMouseButton(0);
        }
        else
        {
            _light2D.enabled = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // If something with the "Enemy" tag enters, initialize the next-attack time
        if (other.CompareTag("Enemy") && !_nextAttackTime.ContainsKey(other))
        {
            _nextAttackTime[other] = 0f;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // Only damage enemies if the light is actually on
        if (!_light2D.enabled) return;

        // Check if this object is an Enemy
        if (other.CompareTag("Enemy"))
        {
            // Get the Enemy script from this collider
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                // Ensure we have a record for the next attack time
                if (!_nextAttackTime.ContainsKey(other))
                {
                    _nextAttackTime[other] = 0f;
                }

                // Check if we are allowed to attack now
                if (Time.time >= _nextAttackTime[other])
                {
                    // Apply damage to the enemy's heatlth field
                    enemy.heatlth -= damage;

                    // If enemy's health is <= 0, destroy it
                    if (enemy.heatlth <= 0)
                    {
                        Destroy(enemy.gameObject);
                    }

                    // Calculate the next time we can deal damage to this enemy
                    _nextAttackTime[other] = Time.time + (1f / attackSpeed);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Remove the enemy from our dictionary when it leaves the light's area
        if (_nextAttackTime.ContainsKey(other))
        {
            _nextAttackTime.Remove(other);
        }
    }
}
