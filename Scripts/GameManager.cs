using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public float speedAmplifier = 1.0f;
    public float healthAmplifier = 1.0f;
    public float energyAmplifier = 1.0f;
    public float attackSpeedAmplifier = 1.0f;
    public float damageAmplifier = 1.0f;
    public int level = 1;
    public int courency = 0;
    public bool isNightmare = false;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Kill duplicates
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Persist through scenes
    }

    void Start()
    {
        // Placeholder: Setup code for initial game state if needed
        Debug.Log("GameManager started. Nightmare mode: " + isNightmare);
    }

    void Update()
    {
        // Optional: Add ticking logic later
    }
}
