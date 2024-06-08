using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeySpawnController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Désactive toutes les clés au cas où
        for (int i = 0; i < transform.childCount; i++)
            transform.GetChild(i).gameObject.SetActive(false);
        // Nombre aléatoire pour la clé
        System.Random random = new System.Random();
        int randomIndex = random.Next(0, transform.childCount);
        // Active la clé choisi aléatoirement
        transform.GetChild(randomIndex).gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
