using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data AI", menuName = "Data/AI")]
public class DataAI : ScriptableObject
{
    public DataPopulation dataPopulation;

    [Header("Data Genome:")] 
    public float[] genomes;
}