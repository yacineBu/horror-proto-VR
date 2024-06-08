using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{

    private void Start()
    {
        GetComponent<TriggerZone>().onEnterEvent.AddListener(insideDoor);
    }

    // On supprime la cl� et on d�verouille les portes d'entr�es
    public void insideDoor(GameObject go)
    {
        if (go.tag == "Key")
        {
            go.SetActive(false);
            FrontDoorController.Instance.UnlockDoor();
        }
    }
}
