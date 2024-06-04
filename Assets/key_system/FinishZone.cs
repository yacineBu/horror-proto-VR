using UnityEngine;
using UnityEngine.SceneManagement; // Assuming you want to load another scene or restart the game

public class FinishZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInventory playerInventory = other.GetComponent<PlayerInventory>();
            if (playerInventory != null && playerInventory.HasKey())
            {
                // Player has the key, finish the game
                Debug.Log("Player has the key! Game Finished.");
                // For example, load the next scene or show a victory message
                // SceneManager.LoadScene("NextSceneName");
            }
            else
            {
                Debug.Log("Player does not have the key.");
            }
        }
    }
}
