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
    public bool onlyOnLeftMouse = false;

    [Header("Attack Settings")]
    public float damage = 5f;
    public float attackSpeed = 1f;

    private Light2D _light2D;
    private Player player;
    private Dictionary<Collider2D, float> _nextAttackTime = new Dictionary<Collider2D, float>();

    private void Awake()
    {
        _light2D = GetComponent<Light2D>();
        player = GetComponentInParent<Player>();

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;
    }

    private void Update()
    {
        // Update visuals
        _light2D.color = lightColor;
        _light2D.intensity = intensity;
        _light2D.pointLightInnerAngle = innerAngle;
        _light2D.pointLightOuterAngle = outerAngle;
        _light2D.pointLightInnerRadius = innerRadius;
        _light2D.pointLightOuterRadius = outerRadius;
        _light2D.falloffIntensity = falloffStrength;

        // Energy check first
        if (player.energy <= 0f)
        {
            _light2D.enabled = false;
            return;
        }

        // Light toggle
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
        if (other.CompareTag("Enemy") && !_nextAttackTime.ContainsKey(other))
        {
            _nextAttackTime[other] = 0f;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!_light2D.enabled || player.energy <= 0f) return;

        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                if (!_nextAttackTime.ContainsKey(other))
                {
                    _nextAttackTime[other] = 0f;
                }

                if (Time.time >= _nextAttackTime[other])
                {
                    enemy.heatlth -= damage;

                    if (enemy.heatlth <= 0)
                    {
                        Destroy(enemy.gameObject);
                    }

                    _nextAttackTime[other] = Time.time + (1f / attackSpeed);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (_nextAttackTime.ContainsKey(other))
        {
            _nextAttackTime.Remove(other);
        }
    }

    public bool GetIsFlashlightOn()
    {
        return _light2D.enabled && (!onlyOnLeftMouse || Input.GetMouseButton(0));
    }

    public void ForceDisable()
    {
        _light2D.enabled = false;
    }
}
