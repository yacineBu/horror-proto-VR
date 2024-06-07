using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using System.Collections.Generic;

public class Door : MonoBehaviour
{
    bool trig, open;
    public float smooth = 2.0f;
    public float DoorOpenAngle = 90.0f;
    private Vector3 defaulRot;
    private Vector3 openRot;

    void Start()
    {
        defaulRot = transform.eulerAngles;
        openRot = new Vector3(defaulRot.x, defaulRot.y + DoorOpenAngle, defaulRot.z);
    }

    void Update()
    {
        if (open)
        {
            transform.eulerAngles = Vector3.Slerp(transform.eulerAngles, openRot, Time.deltaTime * smooth);
        }
        else
        {
            transform.eulerAngles = Vector3.Slerp(transform.eulerAngles, defaulRot, Time.deltaTime * smooth);
        }
        if (Input.GetKeyDown(KeyCode.E) && trig)
        {
            open = !open;
        }

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
            open = !open;
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
