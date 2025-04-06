using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class CountdownTimer : MonoBehaviour
{
    public float startTime = 100f;
    public string sceneToLoad = "Scene";

    public TMP_Text timerText;

    private float currentTime;

    void Start()
    {
        currentTime = startTime;
        UpdateTimerUI();
    }

    void Update()
    {
        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            UpdateTimerUI();

            if (currentTime <= 0f)
            {
                currentTime = 0f;

                // Increase level before changing scene
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.level++;
                    Debug.Log("Level increased! Now at level: " + GameManager.Instance.level);
                }

                SceneManager.LoadScene(sceneToLoad);
            }
        }
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
