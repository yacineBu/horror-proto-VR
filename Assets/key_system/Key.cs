using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{

    private void Start()
    {
        GetComponent<TriggerZone>().onEnterEvent.AddListener(insideDoor);
    }

    // On supprime la clé et on déverouille les portes d'entrées
    public void insideDoor(GameObject go)
    {
        if (go.tag == "Key")
        {
            go.SetActive(false);
            FrontDoorController.Instance.UnlockDoor();
        }
    }
}
