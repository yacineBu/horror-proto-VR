using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class KeyPickup : MonoBehaviour
{
    public InputHelpers.Button activationButton = InputHelpers.Button.Trigger; // Choose the button to use
    public float activationThreshold = 0.1f; // Threshold for button press
    private bool isHovering = false;
    private InputDevice rightHandDevice;
    private PlayerInventory playerInventory;

    private void Start()
    {
        List<InputDevice> devices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller, devices);

        if (devices.Count > 0)
        {
            rightHandDevice = devices[0]; // Assume the first right-hand controller found is the one we want
        }
        else
        {
            Debug.LogWarning("No right-hand controller found!");
        }

        // Find the PlayerInventory component in the player's hierarchy
        playerInventory = FindObjectOfType<PlayerInventory>();
    }

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
        if (isHovering && rightHandDevice.isValid)
        {
            bool isPressed = false;
            rightHandDevice.IsPressed(activationButton, out isPressed, activationThreshold);

            if (isPressed)
            {
                if (playerInventory != null)
                {
                    playerInventory.PickupKey();
                    Destroy(gameObject); // Destroy the key object after picking it up
                }
            }
        }
    }
}
