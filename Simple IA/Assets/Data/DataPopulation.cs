using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DataPopulation : ScriptableObject
{
    public int populationCount = 10;

    public int iterationCount = 1;

    public int eliteCount = 4;
    public float mutationChance = 0.10f;
    public float mutationRate = 0.01f;

    public int inputsCount = 4;
    public int hiddenLayers = 1;
    public int outputsCount = 2;
    public int neuronsCountPerHL = 7;
    public float bias = 1f;
    public float P = 0.5f;
}