using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class KeyPickup : MonoBehaviour
{
    public XRController rightHandController; // Assign the right hand controller in the inspector
    public InputHelpers.Button activationButton = InputHelpers.Button.Trigger; // Choose the button to use

    private bool isHovering = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isHovering = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isHovering = false;
        }
    }

    private void Update()
    {
        if (isHovering && rightHandController)
        {
            if (InputHelpers.IsPressed(rightHandController.inputDevice, activationButton, out bool isPressed) && isPressed)
            {
                PlayerInventory playerInventory = rightHandController.GetComponentInParent<PlayerInventory>();
                if (playerInventory != null)
                {
                    playerInventory.PickupKey();
                    Destroy(gameObject); // Destroy the key object after picking it up
                }
            }
        }
    }
}
