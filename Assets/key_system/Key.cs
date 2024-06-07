using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{

    private void Start()
    {
        GetComponent<TriggerZone>().onEnterEvent.AddListener(insideDoor);
    }

    public void insideDoor(GameObject go)
    {
        go.SetActive(false);
    }
}
