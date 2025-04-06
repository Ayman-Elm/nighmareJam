using UnityEngine;
using UnityEngine.UI;  // For Button
using TMPro;           // For TMP_Text (if you're using TextMeshPro)
using FMODUnity;       // For FMOD functionality
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

    [Header("Manual Upgrade Choice")]
    [Tooltip("Pick which upgrade index you want to offer in this button.")]
    [SerializeField] private int chosenUpgradeIndex = 0;

    // UI references
    public Button btn;        // Assign in Inspector
    public TMP_Text btnText;  // If using TextMeshPro for the button label

    private void Start()
    {
        // If you haven't assigned a separate TMP_Text in the inspector,
        // you could get it from the button's children:
        // btnText = btn.GetComponentInChildren<TMP_Text>();

        // Make sure chosenUpgradeIndex is valid
        if (chosenUpgradeIndex < 0 || chosenUpgradeIndex >= UpgradeList.Length)
        {
            Debug.LogWarning("chosenUpgradeIndex is out of range! Defaulting to 0.");
            chosenUpgradeIndex = 0;
        }

        // Update button text to show which upgrade and cost
        UpdateButtonLabel();

        // Listen for clicks
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

        // Determine the cost of the chosen upgrade
        int cost = UpgradeCosts[chosenUpgradeIndex];

        // Check currency
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

            Debug.Log(
                $"Bought {UpgradeList[chosenUpgradeIndex]} for {cost}. " +
                $"Remaining currency: {GameManager.Instance.courency} | " +
                $"SpeedAmp: {GameManager.Instance.speedAmplifier}"
            );

            // If you want the same button to remain the same upgrade,
            // do nothing else here.
            // If you'd like to switch to a new upgrade automatically,
            // you could re-assign chosenUpgradeIndex or do something else.

            // chosenUpgradeIndex = 0; // or any other logic
            // UpdateButtonLabel();
        }
        else
        {
            Debug.Log("Not enough currency to buy this upgrade.");
        }
    }

    private void UpdateButtonLabel()
    {
        // e.g. "MovementSpeed - $50"
        string upgradeName = UpgradeList[chosenUpgradeIndex];
        int cost = UpgradeCosts[chosenUpgradeIndex];

        // If not using TextMeshPro, do the same with legacy Text
        // if (btnText != null)
        // {
        //     btnText.text = $"{upgradeName} - ${cost}";
        // }
    }

    private void ApplyUpgrade(string upgradeName)
    {
        switch (upgradeName)
        {
            case "MovementSpeed":
                GameManager.Instance.speedAmplifier += 0.1f;
                break;

            case "HealthIncrease":
                GameManager.Instance.healthAmplifier += 0.1f;
                break;

            case "AttackSpeed":
                GameManager.Instance.attackSpeedAmplifier += 0.1f;
                break;

            case "EnergyIncrease":
                GameManager.Instance.energyAmplifier += 0.1f;
                break;

            case "AttackDamage":
                GameManager.Instance.damageAmplifier += 0.1f;
                break;

            case "LightRange":
                Debug.Log("LightRange upgrade applied!");
                // If you have a variable for it, do:
                // GameManager.Instance.lightRange += 1.0f;
                break;

            case "LightWidth":
                Debug.Log("LightWidth upgrade applied!");
                // e.g. GameManager.Instance.lightWidth += 1.0f;
                break;
        }
    }
}
