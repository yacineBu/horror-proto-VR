using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeySpawnController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // D�sactive toutes les cl�s au cas o�
        for (int i = 0; i < transform.childCount; i++)
            transform.GetChild(i).gameObject.SetActive(false);
        // Nombre al�atoire pour la cl�
        System.Random random = new System.Random();
        int randomIndex = random.Next(0, transform.childCount);
        // Active la cl� choisi al�atoirement
        transform.GetChild(randomIndex).gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
