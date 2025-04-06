using UnityEngine;
using UnityEngine.SceneManagement;
using FMODUnity;
using FMOD.Studio;
using System.Collections;

public class BackButton : MonoBehaviour
{
    [Tooltip("Name of the scene to return to")]
    public string returnSceneName = "GameScene"; // Set this in the Inspector

    private void Start()
    {
        // Start the shop music when entering the scene
        RuntimeManager.PlayOneShot("event:/ShopMusic");
    }

    public void GoBack()
    {
        // Stop all instances of the ShopMusic event
        RuntimeManager.StudioSystem.getEvent("event:/ShopMusic", out var eventDescription);
        if (eventDescription.isValid())
        {
            eventDescription.getInstanceList(out var instances);
            foreach (var instance in instances)
            {
                instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                instance.release();
            }
        }

        // Wait a frame to ensure the event is stopped
        StartCoroutine(ChangeSceneAfterDelay());
    }

    private System.Collections.IEnumerator ChangeSceneAfterDelay()
    {
        yield return null; // Wait one frame
        SceneManager.LoadScene(returnSceneName);
    }
}
