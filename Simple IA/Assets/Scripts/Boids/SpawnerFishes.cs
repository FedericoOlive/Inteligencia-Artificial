using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerFishes : MonoBehaviour
{
    [SerializeField] private GameObject pfFish;

    [SerializeField] private int amount = 10;

    void Start()
    {
        for (int i = 0; i < amount; i++)
        {
            Instantiate(pfFish, transform);
        }
    }

    void Update()
    {
        
    }
}