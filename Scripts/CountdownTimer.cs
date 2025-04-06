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
    Time.timeScale = 0f;

    float elapsed = 0f;
    float originalIntensity = flashlightToFlash.intensity;
    float originalRadius = flashlightToFlash.pointLightOuterRadius;
    float originalAngle = flashlightToFlash.pointLightOuterAngle;

    float targetIntensity = targetFlashIntensity;
    float targetRadius = 100f; // 💡 really big to cover screen
    float targetAngle = 360f;  // 🔄 full circle flood

    while (elapsed < flashDuration)
    {
        elapsed += Time.unscaledDeltaTime;
        float t = elapsed / flashDuration;

        flashlightToFlash.intensity = Mathf.Lerp(originalIntensity, targetIntensity, t);
        flashlightToFlash.pointLightOuterRadius = Mathf.Lerp(originalRadius, targetRadius, t);
        flashlightToFlash.pointLightOuterAngle = Mathf.Lerp(originalAngle, targetAngle, t);
        flashlightToFlash.pointLightInnerRadius = flashlightToFlash.pointLightOuterRadius * 0.5f;
        flashlightToFlash.pointLightInnerAngle = flashlightToFlash.pointLightOuterAngle * 0.5f;

        yield return null;
    }

    // Snap to final values just in case
    flashlightToFlash.intensity = targetIntensity;
    flashlightToFlash.pointLightOuterRadius = targetRadius;
    flashlightToFlash.pointLightOuterAngle = targetAngle;
    flashlightToFlash.pointLightInnerRadius = targetRadius * 0.5f;
    flashlightToFlash.pointLightInnerAngle = targetAngle * 0.5f;

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
