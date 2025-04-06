using UnityEngine;
using UnityEngine.SceneManagement;

public class BackButton : MonoBehaviour
{
    [Tooltip("Name of the scene to return to")]
    public string returnSceneName = "GameScene"; // Set this in the Inspector

    public void GoBack()
    {
        SceneManager.LoadScene(returnSceneName);
    }

        public void QuitGame()
    {
        Debug.Log("Quit button pressed!");

        // Works in builds
        Application.Quit();

        // Optional: Quit play mode in the editor
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
