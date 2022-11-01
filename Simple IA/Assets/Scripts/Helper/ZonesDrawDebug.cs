using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZonesDrawDebug : MonoBehaviour
{
    public Transform zoneTanks;
    public Transform zoneMines;

    private void OnDrawGizmos ()
    {
        Gizmos.color = Color.green;


    }
}