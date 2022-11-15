using UnityEngine;
using System.Collections.Generic;

public class PopulationManager : MonoBehaviour
{
    public static event System.Action OnEpoch;

    public GameObject prefabVillager;

    [SerializeField] private DataPopulation dataPopulation;
    //public int PopulationCount = 10;

    public float generationDuration = 20.0f;
    public int iterationCount = 1;
    public int team;

    GeneticAlgorithm genAlg;
    public Village village;

    float accumTime = 0;
    bool isRunning = false;

    public void Init ()
    {

    }

    public void DeInit ()
    {

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
        DestroyTanks();

        village.generation = 0;
        village.SetTeam(team);

        for (int i = 0; i < dataPopulation.populationCount; i++)
        {
            NeuralNetwork brain = CreateBrain();
            Genome genome = new Genome(brain.GetTotalWeightsCount());

            brain.SetWeights(genome.genome);
            village.brains.Add(brain);

            village.population.Add(genome);
            village.populationGOs.Add(CreateTank(genome, brain, i));
        }

        village.SetTanks();
        accumTime = 0.0f;
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
    void Epoch ()
    {
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
            village.populationGOs[i].transform.position = TerrainGenerator.GetSpawnPoints(team)[i].position;
            village.populationGOs[i].transform.rotation = Quaternion.identity;
        }
    }

    void FixedUpdate ()
    {
        if (!isRunning)
            return;

        village.UpdateFitness();

        float dt = Time.fixedDeltaTime;

        for (int i = 0; i < Mathf.Clamp((float) (iterationCount / 100.0f) * 50, 1, 50); i++)
        {

            foreach (Villager tank in village.populationGOs)
            {
                Food food = GetNearestFood(tank.transform.position);
                tank.SetNearestFood(food);
                tank.Think(dt);
            }

            // Check the time to evolve
            accumTime += dt;
            if (accumTime >= generationDuration)
            {
                accumTime -= generationDuration;
                Epoch();
                break;
            }
        }
    }

    #region Helpers

    Villager CreateTank (Genome genome, NeuralNetwork brain, int i)
    {
        Vector3 position = TerrainGenerator.GetSpawnPoints(team)[i].position;
        GameObject go = Instantiate<GameObject>(prefabVillager, position, Quaternion.identity, transform);
        Villager t = go.GetComponent<Villager>();
        t.SetBrain(genome, brain);
        return t;
    }

    void DestroyTanks ()
    {
        foreach (Villager go in village.populationGOs)
            Destroy(go.gameObject);

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