using TMPro;
using UnityEngine;

public class HealthDisplayTMP : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI healthTextTMP;
    [SerializeField] private Player player;  // Reference to your Player script

    private void Update()
    {
        if (player != null && healthTextTMP != null)
        {
            // Format however you like:
            healthTextTMP.text = $"HP: {player.health:F1}";
        }
    }
}
