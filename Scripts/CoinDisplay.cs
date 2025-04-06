using TMPro;
using UnityEngine;

public class CoinDisplayTMP : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinTextTMP;

    void Update()
    {
        if (GameManager.Instance != null)
        {
            // Update the TMP text with the coin amount
            coinTextTMP.text = $": {GameManager.Instance.courency}";
        }
    }
}
