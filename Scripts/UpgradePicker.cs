using UnityEngine;
using UnityEngine.UI;  // For Button
using TMPro;           // For TMP_Text (if you're using TextMeshPro)
using FMODUnity;      // For FMOD functionality
using UnityEngine.EventSystems;  // For EventTrigger

public class UpgradePicker : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private EventReference buttonClickSound;
    [SerializeField] private EventReference buttonHoverSound;

    // List of possible upgrades
    [SerializeField] 
    private string[] UpgradeList = 
    { 
        "MovementSpeed", 
        "HealthIncrease", 
        "AttackSpeed", 
        "EnergyIncrease", 
        "AttackDamage", 
        "LightRange", 
        "LightWidth" 
    };

    // Matching costs for each upgrade (by index)
    [SerializeField] 
    private int[] UpgradeCosts = 
    {
        50,  // MovementSpeed
        100, // HealthIncrease
        75,  // AttackSpeed
        80,  // EnergyIncrease
        120, // AttackDamage
        200, // LightRange
        150  // LightWidth
    };

    // UI references
    public Button btn;        // Assign in Inspector
    public TMP_Text btnText;  // If using TextMeshPro for the button label

    private int chosenUpgradeIndex;

    private void Start()
    {
        // If you haven't assigned a separate TMP_Text in the inspector,
        // you can also get it from the button's children:
        // btnText = btn.GetComponentInChildren<TMP_Text>();

        // Choose an upgrade at Start (randomly)
        chosenUpgradeIndex = UnityEngine.Random.Range(0, UpgradeList.Length);

        // Update button text to show which upgrade we got and how much it costs
        // UpdateButtonLabel();

        // Listen for clicks and hover events
        btn.onClick.AddListener(OnButtonClick);
        
        // Add hover events
        EventTrigger trigger = btn.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = btn.gameObject.AddComponent<EventTrigger>();
        }

        EventTrigger.Entry pointerEnter = new EventTrigger.Entry();
        pointerEnter.eventID = EventTriggerType.PointerEnter;
        pointerEnter.callback.AddListener((data) => OnButtonHover());
        trigger.triggers.Add(pointerEnter);
    }

    private void OnButtonHover()
    {
        if (!buttonHoverSound.IsNull)
        {
            AudioManager.Instance.PlayOneShot(buttonHoverSound);
        }
    }

    private void OnButtonClick()
    {
        // Play click sound
        if (!buttonClickSound.IsNull)
        {
            AudioManager.Instance.PlayOneShot(buttonClickSound);
        }

        int cost = UpgradeCosts[chosenUpgradeIndex];
        if (GameManager.Instance.courency >= cost)
        {
            GameManager.Instance.courency -= cost;

            ApplyUpgrade(UpgradeList[chosenUpgradeIndex]);

            // Apply amplifier values to player + flashlight
            Player player = FindObjectOfType<Player>();
            if (player != null)
            {
                player.ApplyAmplifiers();
            }

            Debug.Log($"Bought {UpgradeList[chosenUpgradeIndex]} for {cost}. " +
                      $"Remaining currency: {GameManager.Instance.courency}" + 
                      $" | SpeedAmp: {GameManager.Instance.speedAmplifier}");

            chosenUpgradeIndex = UnityEngine.Random.Range(0, UpgradeList.Length);
            // UpdateButtonLabel();
        }
    }

    // private void UpdateButtonLabel()
    // {
    //     // e.g. "MovementSpeed - $50"
    //     string upgradeName = UpgradeList[chosenUpgradeIndex];
    //     int cost = UpgradeCosts[chosenUpgradeIndex];

    //     // If not using TextMeshPro, do the same with legacy Text
    //     if (btnText != null)
    //     {
    //         btnText.text = $"{upgradeName} - ${cost}";
    //     }
    // }

    private void ApplyUpgrade(string upgradeName)
    {
        switch (upgradeName)
        {
            case "MovementSpeed":
                // Increase the speedAmplifier
                GameManager.Instance.speedAmplifier += 0.1f;
                break;

            case "HealthIncrease":
                // Increase the healthAmplifier
                GameManager.Instance.healthAmplifier += 0.1f;
                break;

            case "AttackSpeed":
                // Increase attack speed amplifier
                GameManager.Instance.attackSpeedAmplifier += 0.1f;
                break;

            case "EnergyIncrease":
                // Increase the energy amplifier
                GameManager.Instance.energyAmplifier += 0.1f;
                break;

            case "AttackDamage":
                // Increase damage amplifier
                GameManager.Instance.damageAmplifier += 0.1f;
                break;

            case "LightRange":
                // If you have a custom variable or method for this, call it
                // Example: GameManager.Instance.lightRange += 1.0f;
                Debug.Log("LightRange upgrade applied!");
                break;

            case "LightWidth":
                // Another placeholder upgrade effect
                Debug.Log("LightWidth upgrade applied!");
                break;
        }
    }
}
