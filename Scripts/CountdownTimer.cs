using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class CountdownTimer : MonoBehaviour
{
    public float startTime = 100f;
    public string sceneToLoad = "WinScene";
    public TMP_Text timerText;

    public Light2D flashlightToFlash; // 🔦 Reference to the flashlight Light2D
    public float flashDuration = 2f;
    public float targetFlashIntensity = 5f;

    private float currentTime;
    private bool timerDone = false;

    void Start()
    {
        currentTime = startTime;
        UpdateTimerUI();
    }

    void Update()
    {
        if (!timerDone && currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            UpdateTimerUI();

            if (currentTime <= 0f)
            {
                currentTime = 0f;
                timerDone = true;

                if (GameManager.Instance != null)
                {
                    GameManager.Instance.level++;
                }

                StartCoroutine(FlashAndLoad());
            }
        }
    }

    IEnumerator FlashAndLoad()
    {
        // 🛑 PAUSE: Disable player & enemy movement if needed
        Time.timeScale = 0f;

        float elapsed = 0f;
        float originalIntensity = flashlightToFlash.intensity;

        // Slowly brighten the flashlight over flashDuration (in real time)
        while (elapsed < flashDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / flashDuration;
            flashlightToFlash.intensity = Mathf.Lerp(originalIntensity, targetFlashIntensity, t);
            yield return null;
        }

        // Just in case it didn’t reach max exactly
        flashlightToFlash.intensity = targetFlashIntensity;

        // ⚠️ Unpause before loading
        Time.timeScale = 1f;

        SceneManager.LoadScene(sceneToLoad);
    }

    void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(currentTime / 60);
            int seconds = Mathf.FloorToInt(currentTime % 60);
            timerText.text = $"{minutes:00}:{seconds:00}";
        }
    }
}
