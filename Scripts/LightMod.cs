using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using FMODUnity;
using FMOD.Studio;

[RequireComponent(typeof(Light2D))]
[RequireComponent(typeof(Collider2D))]
public class LightMod : MonoBehaviour
{
    [SerializeField] public EventReference flashlightOnSound;
    [SerializeField] public EventReference flashlightOffSound;

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

    private float baseDamage;
    private float baseAttackSpeed;

    private Light2D _light2D;
    private bool _wasLightEnabled = false;
    private EventInstance _flashlightSound;

    private Player player;
    private Dictionary<Collider2D, float> _nextAttackTime = new Dictionary<Collider2D, float>();

    private void Awake()
    {
        _light2D = GetComponent<Light2D>();
        _flashlightSound = RuntimeManager.CreateInstance(flashlightOnSound);
        player = GetComponentInParent<Player>();

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;
    }

    private void Start()
    {
        baseDamage = damage;
        baseAttackSpeed = attackSpeed;

        ApplyAmplifiersFromGameManager(); // Apply initial values
    }

    public void ApplyAmplifiersFromGameManager()
    {
        if (GameManager.Instance != null)
        {
            damage = baseDamage * GameManager.Instance.damageAmplifier;
            attackSpeed = baseAttackSpeed * GameManager.Instance.attackSpeedAmplifier;
        }
    }

    private void OnDestroy()
    {
        if (_flashlightSound.isValid())
        {
            _flashlightSound.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            _flashlightSound.release();
        }
    }

    private void Update()
    {
        // Update light visuals
        _light2D.color = lightColor;
        _light2D.intensity = intensity;
        _light2D.pointLightInnerAngle = innerAngle;
        _light2D.pointLightOuterAngle = outerAngle;
        _light2D.pointLightInnerRadius = innerRadius;
        _light2D.pointLightOuterRadius = outerRadius;
        _light2D.falloffIntensity = falloffStrength;

        // Don't allow light if out of energy
        if (player.energy <= 0f)
        {
            _light2D.enabled = false;
            return;
        }

        // Light control: hold-to-use logic
        if (onlyOnLeftMouse)
        {
            bool shouldBeEnabled = Input.GetMouseButton(0);

            if (shouldBeEnabled && !_wasLightEnabled)
            {
                _flashlightSound.set3DAttributes(RuntimeUtils.To3DAttributes(transform));
                _flashlightSound.start();
            }
            else if (!shouldBeEnabled && _wasLightEnabled)
            {
                _flashlightSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                if (!flashlightOffSound.IsNull)
                {
                    AudioManager.Instance.PlayOneShot(flashlightOffSound, this.transform.position);
                }
            }

            _light2D.enabled = shouldBeEnabled;
            _wasLightEnabled = shouldBeEnabled;
        }
        else
        {
            _light2D.enabled = true;
            _wasLightEnabled = true;
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
                if (!_nextAttackTime.ContainsKey(other))
                {
                    _nextAttackTime[other] = 0f;
                }

                if (Time.time >= _nextAttackTime[other])
                {
                    enemy.heatlth -= damage;

                    if (enemy.heatlth <= 0)
                    {
                        Destroy(enemy.gameObject); // Use Die method to reward currency
                    }

                    _nextAttackTime[other] = Time.time + (1f / attackSpeed);
                }
            }

            if (Time.time >= _nextAttackTime[other])
            {
                // Apply damage
                enemy.heatlth -= damage;

                // Slow the enemy for 2 seconds at half speed
                EnemyAI enemyAI = other.GetComponent<EnemyAI>();
                if (enemyAI != null)
                {
                    enemyAI.ApplySlow();
                } 

                // If the enemy dies...
                if (enemy.heatlth <= 0)
                {
                    Destroy(enemy.gameObject);
                    GameManager.Instance.courency += enemy.CoinDrop;
                }

                _nextAttackTime[other] = Time.time + (1f / attackSpeed);
            }
        } 
    
    }
}

    private void OnTriggerExit2D(Collider2D other)
{
    if (other.CompareTag("Enemy"))
    {
        // Remove timing entry if you like
        if (_nextAttackTime.ContainsKey(other))
        {
            _nextAttackTime.Remove(other);
        }

        // Reset speed stats
        EnemyAI enemyAI = other.GetComponent<EnemyAI>();
        if (enemyAI != null)
        {
            enemyAI.resetStats();
        }
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
