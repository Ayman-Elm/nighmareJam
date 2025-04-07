using TMPro;
using UnityEngine;

public class EnergyDisplayTMP : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI energyTextTMP;
    [SerializeField] private Player player;  // Reference to the Player script

    private void Update()
    {
        if (player != null && energyTextTMP != null)
        {
            // Show current energy with 1 decimal place
            energyTextTMP.text = $":{player.energy:F1}";
        }
    }
}
