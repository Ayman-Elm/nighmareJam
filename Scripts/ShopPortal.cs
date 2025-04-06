using UnityEngine;
using UnityEngine.SceneManagement;
using FMODUnity;
using FMOD.Studio;

public class ShopPortal : MonoBehaviour
{
    public string shopSceneName = "Shop Scene"; // name of your shop scene
    private bool playerInRange = false;

    private void Start()
    {
        // Start the room music when entering the scene
        RuntimeManager.PlayOneShot("event:/RoomMusic");
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            // Stop all instances of the RoomMusic event
            RuntimeManager.StudioSystem.getEvent("event:/RoomMusic", out var eventDescription);
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
    }

    private System.Collections.IEnumerator ChangeSceneAfterDelay()
    {
        yield return null; // Wait one frame
        SceneManager.LoadScene(shopSceneName);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            // Optional: show "Press E" UI here
            Debug.Log("Player entered shop portal zone. Press E to enter.");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            // Optional: hide "Press E" UI here
            Debug.Log("Player left the shop portal zone.");
        }
    }
}
