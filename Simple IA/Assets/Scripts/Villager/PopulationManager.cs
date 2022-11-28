using System.Collections.Generic;
using UnityEngine;

public class PopulationManager : MonoBehaviour
{
    public GameObject prefabVillager;

    [SerializeField] private DataPopulation dataPopulation;
    [SerializeField] private LevelSettings levelSettings;
    public Team team;

    GeneticAlgorithm genAlg;
    public Village village;
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
    public void GenerateInitialPopulation ()
    {
        // Destroy previous tanks (if there are any)
        DestroyVillagers();

        village.generation = 0;
        village.SetColorCivTeam(team);

        for (int i = 0; i < dataPopulation.populationCount; i++)
        {
            NeuralNetwork brain = CreateBrain();
            Genome genome = new Genome(brain.GetTotalWeightsCount());

            SetVillager(brain, genome, i);
        }

        village.SetVillager();
    }

    public void GenerateInitialPopulation (List<Genome> genomes)
    {
        // Destroy previous tanks (if there are any)
        DestroyVillagers();

        village.generation = 0;
        village.SetColorCivTeam(team);

        for (int i = 0; i < dataPopulation.populationCount; i++)
        {
            NeuralNetwork brain = CreateBrain();
            Genome genome = genomes[i];

            SetVillager(brain, genome, i);
        }

        village.SetVillager();
    }
    
    public void SetVillager (NeuralNetwork brain, Genome genome, int i)
    {
        brain.SetWeights(genome.genome);
        village.brains.Add(brain);
        village.population.Add(genome);
        village.populationGOs.Add(CreateVillager(genome, brain, i));
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
        // Increment generation counter
        village.generation++;

        // Se reproducen solo los que hayan comido más de 1 comida
        List<Genome> populationReproduce = new List<Genome>();
        List<Genome> populationSurvival = new List<Genome>();

        for (int i = village.populationGOs.Count - 1; i >= 0; i--)
        {
            if (village.populationGOs[i].generationsAlive > 0)
            {
                if (village.populationGOs[i].foodsEatsInGeneration > 1)
                {
                    populationReproduce.Add(village.population[i]);
                }
                else if (village.populationGOs[i].foodsEatsInGeneration > 0)
                {
                    populationSurvival.Add(village.population[i]);
                }
            }

            village.populationGOs[i].foodsEatsInGeneration = 0;
        }

        // Clear current population
        village.population.Clear();

        List<Genome> newGenomes = genAlg.Epoch(populationReproduce.ToArray());
        
        // Add new population
        newGenomes.AddRange(populationSurvival);
        village.population.AddRange(newGenomes);
        
        for (int i = 0; i < village.population.Count; i++)
            village.population[i].fitness = 1;
        
        // Set the new genomes as each NeuralNetwork weights
        int j = 0;
        for (; j < newGenomes.Count; j++)
        {
            NeuralNetwork brain = village.brains[j]; // Todo: Crash por combate (Eliminacion o agregacion de cerebro)
            brain.SetWeights(newGenomes[j].genome);
            village.populationGOs[j].SetBrain(newGenomes[j], brain);
            village.populationGOs[j].transform.position = TerrainGenerator.GetSpawnPoints((int) team)[j].position;
            village.populationGOs[j].transform.rotation = Quaternion.identity;
        }

        for (int k = village.populationGOs.Count - 1; k >= j; k--)
        {
            Destroy(village.populationGOs[k].gameObject);
            village.populationGOs.RemoveAt(k);
            village.brains.RemoveAt(k);
        }

        for (int i = populationReproduce.Count; i < newGenomes.Count; i++)
        {
            village.populationGOs[i].generationsAlive = levelSettings.maxGeneationsAlive;
        }
    }

    void EpochWithOutFitness ()
    {
        //village.population.Clear();
        //
        //List<Genome> newGenomes = genAlg.Epoch(village.population.ToArray());

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
    }

    

    #region Helpers

    public Villager CreateVillager (Genome genome, NeuralNetwork brain, int i)
    {
        Vector3 position = TerrainGenerator.GetSpawnPoints((int) team)[i].position;
        GameObject go = Instantiate(prefabVillager, position, Quaternion.identity, transform);
        Villager villager = go.GetComponent<Villager>();
        villager.SetBrain(genome, brain);
        villager.team = team;
        return villager;
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