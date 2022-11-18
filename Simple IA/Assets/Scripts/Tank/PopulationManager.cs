using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PopulationManager : MonoBehaviour
{
    public static event System.Action OnEpoch;

    public GameObject prefabVillager;

    [SerializeField] private DataPopulation dataPopulation;
    [SerializeField] private LevelSettings levelSettings;
    public Team team;

    GeneticAlgorithm genAlg;
    public Village village;
    public float accumTime;
    public float accumRounds;
    bool isRunning;

    public void Init ()
    {
        StartSimulation();
    }

    public void DeInit ()
    {
        DestroyVillagers();
    }

    public void StartSimulation ()
    {
        // Create and confiugre the Genetic Algorithm
        genAlg = new GeneticAlgorithm(dataPopulation.eliteCount, dataPopulation.mutationChance, dataPopulation.mutationRate);

        GenerateInitialPopulation();

        isRunning = true;
    }

    public void PauseSimulation ()
    {
        isRunning = !isRunning;
    }

    // Generate the random initial population
    void GenerateInitialPopulation ()
    {
        // Destroy previous tanks (if there are any)
        DestroyVillagers();

        village.generation = 0;
        village.SetColorCivTeam(team);

        for (int i = 0; i < dataPopulation.populationCount; i++)
        {
            NeuralNetwork brain = CreateBrain();
            Genome genome = new Genome(brain.GetTotalWeightsCount());

            brain.SetWeights(genome.genome);
            village.brains.Add(brain);

            village.population.Add(genome);
            village.populationGOs.Add(CreateVillager(genome, brain, i));

        }

        village.SetVillager();
        accumTime = 0;
    }

    // Creates a new NeuralNetwork
    NeuralNetwork CreateBrain ()
    {
        NeuralNetwork brain = new NeuralNetwork();

        // Add first neuron layer that has as many neurons as inputs
        brain.AddFirstNeuronLayer(dataPopulation.inputsCount, dataPopulation.bias, dataPopulation.p);

        for (int i = 0; i < dataPopulation.hiddenLayers; i++)
        {
            // Add each hidden layer with custom neurons count
            brain.AddNeuronLayer(dataPopulation.neuronsCountPerHL, dataPopulation.bias, dataPopulation.p);
        }

        // Add the output layer with as many neurons as outputs
        brain.AddNeuronLayer(dataPopulation.outputsCount, dataPopulation.bias, dataPopulation.p);

        return brain;
    }

    // Evolve!!!
    public void Epoch ()
    {
        for (int i = 0; i < village.populationGOs.Count; i++)
        {
            village.populationGOs[i].generationsAlive--;
            if (village.populationGOs[i].generationsAlive <= 0)
                village.populationGOs[i].Kill();

        }

        accumTime = 0;
        accumRounds = 0;
        OnEpoch?.Invoke();

        // Increment generation counter
        village.generation++;

        // Evolve each genome and create a new array of genomes
        Genome[] newGenomes = genAlg.Epoch(village.population.ToArray());

        // Clear current population
        village.population.Clear();

        // Add new population
        village.population.AddRange(newGenomes);

        // Set the new genomes as each NeuralNetwork weights
        for (int i = 0; i < dataPopulation.populationCount; i++)
        {
            NeuralNetwork brain = village.brains[i];

            brain.SetWeights(newGenomes[i].genome);

            village.populationGOs[i].SetBrain(newGenomes[i], brain);
            village.populationGOs[i].transform.position = TerrainGenerator.GetSpawnPoints((int) team)[i].position;
            village.populationGOs[i].transform.rotation = Quaternion.identity;
        }
    }

    public void FixedUpdateForGameManager (float deltaTime)
    {
        if (!isRunning)
            return;

        village.UpdateFitness();

        foreach (Villager villager in village.populationGOs)
        {
            Food food = GetNearestFood(villager.transform.position);
            villager.SetNearestFood(food);
            villager.Think(deltaTime);
        }

        accumRounds++;
        accumTime += deltaTime;
    }

    public void CheckForEpoch ()
    {
        switch (levelSettings.generationEndType)
        {
            case GenerationEndType.Manual:
                break;
            case GenerationEndType.Time:
                if (accumTime > levelSettings.timeGenerationDuration)
                    Epoch();
                break;
            case GenerationEndType.Rounds:
                if (accumRounds > levelSettings.roundsGenerationDuration)
                    Epoch();
                break;
        }
    }

    #region Helpers

    Villager CreateVillager (Genome genome, NeuralNetwork brain, int i) // Todo: Acá se crea y setea el villager Inicial
    {
        Vector3 position = TerrainGenerator.GetSpawnPoints((int) team)[i].position;
        GameObject go = Instantiate(prefabVillager, position, Quaternion.identity, transform);
        Villager t = go.GetComponent<Villager>();
        t.SetBrain(genome, brain);
        return t;
    }

    void DestroyVillagers ()
    {
        if (!Application.isPlaying)
            for (int i = 0; i < village.populationGOs.Count; i++)
                DestroyImmediate(village.populationGOs[i].gameObject);
        else
            for (int i = 0; i < village.populationGOs.Count; i++)
                Destroy(village.populationGOs[i].gameObject);

        village.populationGOs.Clear();
        village.population.Clear();
        village.brains.Clear();
    }

    public Vector3 GetRandomPosInBounds (Bounds bounds)
    {
        float minX = bounds.center.x - bounds.extents.x;
        float minZ = bounds.center.z - bounds.extents.z;
        float maxX = bounds.center.x + bounds.extents.x;
        float maxZ = bounds.center.z + bounds.extents.z;
        Vector3 pos = Vector3.zero;
        pos.x = Random.Range(minX, maxX);
        pos.y = 0;
        pos.z = Random.Range(minZ, maxZ);

        return pos;
    }

    Food GetNearestFood (Vector3 pos) => GameManager.Get().GetNearFood(pos);

    private void OnDrawGizmos ()
    {
        village.OnDrawGizmos();
    }

    #endregion

}

public enum Team
{
    Red,
    Magenta,
    Blue,
    Green,
}

public enum GenerationEndType
{
    Manual,
    Time,
    Rounds
}