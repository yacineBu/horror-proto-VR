using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using System.Collections.Generic;

public class FrontDoorController : MonoBehaviour
{
    [SerializeField] GameObject[] doors;
    public float smooth = 2.0f;
    public float DoorOpenAngle = 90.0f;
    private Vector3 defaulRot;
    private Vector3 openRot;
    [HideInInspector] public bool open;
    [HideInInspector] public bool doorUnlocked;

    public static FrontDoorController Instance;

    void Start()
    {
        defaulRot = transform.eulerAngles;
        openRot = new Vector3(defaulRot.x, defaulRot.y + DoorOpenAngle, defaulRot.z);
        doorUnlocked = false;
        Instance = this;
    }

    void Update()
    {
        // Ouvre ou ferme les 2 portes simultanément
        if (doorUnlocked)
        {
            if (open)
            {
                foreach(GameObject door in doors)
                    door.transform.eulerAngles = Vector3.Slerp(transform.eulerAngles, openRot, Time.deltaTime * smooth);
            }
            else
            {
                foreach (GameObject door in doors)
                    door.transform.eulerAngles = Vector3.Slerp(transform.eulerAngles, defaulRot, Time.deltaTime * smooth);
            }
        }
    }

    public void UnlockDoor()
    {
        doorUnlocked = true;
    }
}
