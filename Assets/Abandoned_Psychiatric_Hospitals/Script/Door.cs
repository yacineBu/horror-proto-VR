using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using System.Collections.Generic;

public class Door : MonoBehaviour
{

    bool trig, open;//trig-проверка входа выхода в триггер(игрок должен быть с тегом Player) open-закрыть и открыть дверь
    public float smooth = 2.0f;//скорость вращения
    public float DoorOpenAngle = 90.0f;//угол вращения 
    private Vector3 defaulRot;
    private Vector3 openRot;
    public Text txt;//text 
    // Start is called before the first frame update
    void Start()
    {
        defaulRot = transform.eulerAngles;
        openRot = new Vector3(defaulRot.x, defaulRot.y + DoorOpenAngle, defaulRot.z);
    }

    // Update is called once per frame
    void Update()
    {
        if (open)//открыть
        {
            transform.eulerAngles = Vector3.Slerp(transform.eulerAngles, openRot, Time.deltaTime * smooth);
        }
        else//закрыть
        {
            transform.eulerAngles = Vector3.Slerp(transform.eulerAngles, defaulRot, Time.deltaTime * smooth);
        }
        if (Input.GetKeyDown(KeyCode.E) && trig)
        {
            open = !open;
        }

        /*var leftHand = InputSystem.GetDevice<XRController>(CommonUsages.LeftHand);
        var rightHand = InputSystem.GetDevice<XRController>(CommonUsages.RightHand);

        if (leftHand != null && rightHand != null)
        {
            if ((leftHand.device.TryGetFeatureValue(CommonUsages.triggerButton, out bool leftTriggerPressed) && leftTriggerPressed) ||
                (rightHand.device.TryGetFeatureValue(CommonUsages.triggerButton, out bool rightTriggerPressed) && rightTriggerPressed))
            {
                if (trig)
                {
                    open = !open;
                }
            }
        }*/

        bool leftTriggerPressed = false;
        bool rightTriggerPressed = false;

        var leftHandDevices = new List<UnityEngine.XR.InputDevice>();
        var rightHandDevices = new List<UnityEngine.XR.InputDevice>();
        UnityEngine.XR.InputDevices.GetDevicesAtXRNode(UnityEngine.XR.XRNode.LeftHand, leftHandDevices);
        UnityEngine.XR.InputDevices.GetDevicesAtXRNode(UnityEngine.XR.XRNode.RightHand, rightHandDevices);

        foreach (var device in leftHandDevices)
        {
            device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out leftTriggerPressed);
            if (leftTriggerPressed)
                Debug.Log("LEFT : " + leftTriggerPressed);
            if (leftTriggerPressed) break;
        }

        foreach (var device in rightHandDevices)
        {
            device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out rightTriggerPressed);
            if (rightTriggerPressed)
                Debug.Log("RIGHT : " + rightTriggerPressed);
            if (rightTriggerPressed) break;
        }

        if ((leftTriggerPressed || rightTriggerPressed) && trig)
        {
            open = !open;
        }

        if (trig)
        {
            if (open)
            {
                //txt.text = "Close E";
            }
            else
            {
                //txt.text = "Open E";
            }
        }
    }
    private void OnTriggerEnter(Collider coll)//вход и выход в\из  триггера 
    {
        Debug.Log("OnTriggerEnter");
        if (coll.tag == "Player")
        {
            Debug.Log("oui");
            if (!open)
            {
                //txt.text = "Close E ";
            }
            else
            {
                //txt.text = "Open E";
            }
            trig = true;
        }
    }
    private void OnTriggerExit(Collider coll)//вход и выход в\из  триггера 
    {
        Debug.Log("OnTriggerExit");
        if (coll.tag == "Player")
        {
            Debug.Log("oui");
            //txt.text = " ";
            trig = false;
        }
    }
}
