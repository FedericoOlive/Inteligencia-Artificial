using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

public class LoadAndSaveGenome : MonoBehaviour
{
    [SerializeField] private PopulationManager populationManager;
    [SerializeField] private Button btnSaveGenome;
    [SerializeField] private Button btnLoadGenome;

    void Start ()
    {
        btnSaveGenome.onClick.AddListener(SaveGenome);
        btnLoadGenome.onClick.AddListener(LoadGenome);
    }

    private void SaveGenome ()
    {
        SaveStructure saveStructure = new SaveStructure
        {
            inputsCount = populationManager.InputsCount,
            hiddenLayers = populationManager.HiddenLayers,
            outputsCount = populationManager.OutputsCount,
            neuronsCountPerHl = populationManager.NeuronsCountPerHL,
            bias = populationManager.Bias,
            sigmoid = populationManager.Sigmoid,
            population = populationManager.population,
        };

        Save(saveStructure);
    }

    private void LoadGenome ()
    {
        SaveStructure saveStructure = Load();
        populationManager.InputsCount = saveStructure.inputsCount;
        populationManager.HiddenLayers = saveStructure.hiddenLayers;
        populationManager.OutputsCount = saveStructure.outputsCount;
        populationManager.NeuronsCountPerHL = saveStructure.neuronsCountPerHl;
        populationManager.Bias = saveStructure.bias;
        populationManager.Sigmoid = saveStructure.sigmoid;
        populationManager.population = saveStructure.population;

        populationManager.GenerateInitialPopulation(saveStructure.population);
        populationManager.StartSimulation();
    }

    public static void Save (SaveStructure structure)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/Save.save";
        FileStream stream = new FileStream(path, FileMode.Create);

        SaveStructure data = new SaveStructure(structure);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static SaveStructure Load ()
    {
        string path = Application.persistentDataPath + "/Save.save";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            SaveStructure data = formatter.Deserialize(stream) as SaveStructure;
            stream.Close();

            return data;
        }
        else
        {
            Debug.Log("Save FIle not Found");
            return null;
        }
    }
}

[System.Serializable]
public class SaveStructure
{
    public int inputsCount;
    public int hiddenLayers;
    public int outputsCount;
    public int neuronsCountPerHl;
    public float bias;
    public float sigmoid;

    public List<Genome> population;

    public SaveStructure ()
    {
    }

    public SaveStructure (SaveStructure structure)
    {

    }
}