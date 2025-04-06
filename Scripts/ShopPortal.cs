using UnityEngine;
using UnityEngine.SceneManagement;

public class ShopPortal : MonoBehaviour
{
    public string shopSceneName = "Shop Scene"; // name of your shop scene
    private bool playerInRange = false;

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            // Load the shop scene
            SceneManager.LoadScene(shopSceneName);
        }
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
