using System.Collections.Generic;
using UnityEngine;

public class PopulationManager : MonoBehaviour
{
    public GameObject prefabVillager;
    public bool drawGizmo;
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
            Genome genome;
            if (i >= genomes.Count)
            {
                genome = new Genome(brain.GetTotalWeightsCount());
            }
            else
            {
                genome = genomes[i];
            }

            SetVillager(brain, genome, i);
        }

        village.SetVillager();
    }

    public void SetVillager (NeuralNetwork brain, Genome genome, int i)
    {
        brain.SetWeights(genome.genome);
        CreateVillager(genome, brain, i);
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
        village.generation++;
        List<VillagerData> villagerSurvival = new List<VillagerData>();

        // Se reproducen solo los que hayan comido más de 1 comida
        List<Genome> populationReproduce = new List<Genome>();

        for (int i = village.populationGOs.Count - 1; i >= 0; i--)
        {
            if (village.populationGOs[i].villagerData.generationsAlive > 0)
            {
                if (village.populationGOs[i].foodsEatsInGeneration > 1)
                {
                    populationReproduce.Add(village.population[i]);
                }

                if (village.populationGOs[i].foodsEatsInGeneration > 0)
                {
                    villagerSurvival.Add(village.populationGOs[i].villagerData);
                }

            }

            village.populationGOs[i].foodsEatsInGeneration = 0;
        }

        DestroyVillagers();
        List<Genome> newGenomes = genAlg.Epoch(populationReproduce.ToArray());

        for (int i = 0; i < villagerSurvival.Count; i++)
        {
            Villager vill = CreateVillager(villagerSurvival[i].genome, villagerSurvival[i].brain, i);
            vill.villagerData = villagerSurvival[i];
        }

        for (int i = 0; i < newGenomes.Count; i++)
        {
            CreateVillager(newGenomes[i], null, villagerSurvival.Count + i);
        }

        village.SetVillager();
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
        if (brain == null)
        {
            brain = CreateBrain();
        }

        if (i > TerrainGenerator.GetSpawnPoints((int) team).Count - 1)
            i = 0;

        if (i < 0) i = 0;
        Vector3 position = TerrainGenerator.GetSpawnPoints((int) team)[i].position;
        GameObject go = Instantiate(prefabVillager, position, Quaternion.identity, transform);
        Villager villager = go.GetComponent<Villager>();
        villager.SetBrain(genome, brain);
        villager.team = team;
        
        village.brains.Add(brain);
        village.population.Add(genome);
        village.populationGOs.Add(villager);
        village.SetColorVillager(villager);

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

#if UNITY_EDITOR
    private void OnDrawGizmos ()
    {
        if (drawGizmo)
            village.OnDrawGizmos();
    }
#endif
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

public class VillagerData
{
    public Genome genome;
    public NeuralNetwork brain;
    public int life;
    public int generationsAlive = 3;
}