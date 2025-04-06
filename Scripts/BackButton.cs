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
}
