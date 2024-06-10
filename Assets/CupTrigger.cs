using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CupTrigger : MonoBehaviour
{
    public Rigidbody cup;
    private void OnTriggerEnter(Collider other)
    {
        cup.AddForce(Vector3.right * 2f, ForceMode.Impulse);
    }
}
