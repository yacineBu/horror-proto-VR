using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using System.Collections.Generic;

public class FrontDoor : MonoBehaviour
{
    bool trig;

    void Start()
    {
        trig = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(FrontDoorController.Instance.doorUnlocked)
        {
            if (Input.GetKeyDown(KeyCode.E) && trig)
            {
                FrontDoorController.Instance.open = !FrontDoorController.Instance.open;
            }

            // Si la porte est déverouillé, on check la collision et la grip 
            bool leftTriggerPressed = false;
            bool rightTriggerPressed = false;

            var leftHandDevices = new List<UnityEngine.XR.InputDevice>();
            var rightHandDevices = new List<UnityEngine.XR.InputDevice>();
            UnityEngine.XR.InputDevices.GetDevicesAtXRNode(UnityEngine.XR.XRNode.LeftHand, leftHandDevices);
            UnityEngine.XR.InputDevices.GetDevicesAtXRNode(UnityEngine.XR.XRNode.RightHand, rightHandDevices);

            foreach (var device in leftHandDevices)
            {
                device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out leftTriggerPressed);
                if (leftTriggerPressed) break;
            }

            foreach (var device in rightHandDevices)
            {
                device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out rightTriggerPressed);
                if (rightTriggerPressed) break;
            }

            if ((leftTriggerPressed || rightTriggerPressed) && trig)
            {
                // Change l'état des portes
                FrontDoorController.Instance.open = !FrontDoorController.Instance.open;
            }
        }
    }
    private void OnTriggerEnter(Collider coll)
    {
        if (coll.tag == "Player")
        {
            trig = true;
        }
    }
    private void OnTriggerExit(Collider coll)
    {
        if (coll.tag == "Player")
        {
            trig = false;
        }
    }
}
