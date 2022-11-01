using UnityEngine;
using System.Collections.Generic;

public class PopulationManager : MonoBehaviour
{
    public static event System.Action OnEpoch;
    public Collider zoneTanks;
    public Collider zoneMines;

    public GameObject TankPrefab;
    public GameObject MinePrefab;

    public int villageCount = 2;
    public int PopulationCount = 10;
    public int MinesCount = 50;

    public Vector3 SceneHalfExtents = new Vector3(20.0f, 0.0f, 20.0f);

    public float GenerationDuration = 20.0f;
    public int IterationCount = 1;

    public int EliteCount = 4;
    public float MutationChance = 0.10f;
    public float MutationRate = 0.01f;

    public int InputsCount = 4;
    public int HiddenLayers = 1;
    public int OutputsCount = 2;
    public int NeuronsCountPerHL = 7;
    public float Bias = 1f;
    public float P = 0.5f;


    GeneticAlgorithm genAlg;

    public List<Village> village = new List<Village>();

    List<Mine> mines = new List<Mine>();

    float accumTime = 0;
    bool isRunning = false;
    
    private float getBestFitness (int index)
    {
        float fitness = 0;

        foreach (Genome g in village[index].population)
        {
            if (fitness < g.fitness)
                fitness = g.fitness;
        }

        return fitness;
    }

    private float getAvgFitness (int index)
    {
        float fitness = 0;
        foreach (Genome g in village[index].population)
        {
            fitness += g.fitness;
        }

        return fitness / village[index].population.Count;
    }

    private float getWorstFitness (int index)
    {
        float fitness = float.MaxValue;
        foreach (Genome g in village[index].population)
        {
            if (fitness > g.fitness)
                fitness = g.fitness;
        }

        return fitness;
    }

    static PopulationManager instance = null;

    public static PopulationManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<PopulationManager>();

            return instance;
        }
    }

    void Awake ()
    {
        instance = this;
    }

    public void StartSimulation ()
    {
        // Create and confiugre the Genetic Algorithm
        genAlg = new GeneticAlgorithm(EliteCount, MutationChance, MutationRate);

        GenerateInitialPopulation();
        CreateMines();

        isRunning = true;
    }

    public void PauseSimulation ()
    {
        isRunning = !isRunning;
    }

    public void StopSimulation ()
    {
        isRunning = false;

        for (int i = 0; i < village.Count; i++)
        {
            village[i].generation = 0;
        }

        // Destroy previous tanks (if there are any)
        DestroyTanks();

        // Destroy all mines
        DestroyMines();
    }

    // Generate the random initial population
    void GenerateInitialPopulation ()
    {
        // Destroy previous tanks (if there are any)
        DestroyTanks();

        for (int i = 0; i < villageCount; i++)
        {
            village.Add(new Village());
            village[i].generation = 0;
            village[i].SetTeam(i);

            for (int j = 0; j < PopulationCount; j++)
            {
                NeuralNetwork brain = CreateBrain();

                Genome genome = new Genome(brain.GetTotalWeightsCount());

                brain.SetWeights(genome.genome);
                village[i].brains.Add(brain);

                village[i].population.Add(genome);
                village[i].populationGOs.Add(CreateTank(genome, brain));
            }

            village[i].SetTanks();
        }

        accumTime = 0.0f;
    }

    // Creates a new NeuralNetwork
    NeuralNetwork CreateBrain ()
    {
        NeuralNetwork brain = new NeuralNetwork();

        // Add first neuron layer that has as many neurons as inputs
        brain.AddFirstNeuronLayer(InputsCount, Bias, P);

        for (int i = 0; i < HiddenLayers; i++)
        {
            // Add each hidden layer with custom neurons count
            brain.AddNeuronLayer(NeuronsCountPerHL, Bias, P);
        }

        // Add the output layer with as many neurons as outputs
        brain.AddNeuronLayer(OutputsCount, Bias, P);

        return brain;
    }

    // Evolve!!!
    void Epoch ()
    {
        OnEpoch?.Invoke();
        int indexBestVillage = 0;
        float maxFitness = 0;

        for (int i = 0; i < village.Count; i++)
        {
            float currentFitness = getBestFitness(i);
            if (currentFitness > maxFitness)
            {
                indexBestVillage = i;
                maxFitness = currentFitness;
            }
        }

        for (int i = 0; i < village.Count; i++)
        {
            if (i == indexBestVillage)
            {
                for (int j = 0; j < PopulationCount; j++)
                {
                    village[i].populationGOs[j].transform.position = GetRandomPosInBounds(zoneTanks.bounds);
                    village[i].populationGOs[j].transform.rotation = GetRandomRot();
                    village[i].bestFitness = getBestFitness(i);
                    village[i].avgFitness = getAvgFitness(i);
                    village[i].worstFitness = getWorstFitness(i);
                    village[i].ResetFitness();
                }

                continue;
            }

            // Increment generation counter
            village[i].generation++;

            // Calculate best, average and worst fitness
            village[i].bestFitness = getBestFitness(i);
            village[i].avgFitness = getAvgFitness(i);
            village[i].worstFitness = getWorstFitness(i);

            // Evolve each genome and create a new array of genomes
            Genome[] newGenomes = genAlg.Epoch(village[i].population.ToArray());

            // Clear current population
            village[i].population.Clear();

            // Add new population
            village[i].population.AddRange(newGenomes);

            // Set the new genomes as each NeuralNetwork weights
            for (int j = 0; j < PopulationCount; j++)
            {
                NeuralNetwork brain = village[i].brains[j];

                brain.SetWeights(newGenomes[j].genome);

                village[i].populationGOs[j].SetBrain(newGenomes[j], brain);
                village[i].populationGOs[j].transform.position = GetRandomPosInBounds(zoneTanks.bounds);
                village[i].populationGOs[j].transform.rotation = GetRandomRot();
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate ()
    {
        if (!isRunning)
            return;

        float dt = Time.fixedDeltaTime;

        for (int i = 0; i < Mathf.Clamp((float) (IterationCount / 100.0f) * 50, 1, 50); i++)
        {
            for (int j = 0; j < village.Count; j++)
            {
                foreach (Tank t in village[j].populationGOs)
                {
                    // Get the nearest mine
                    GameObject mine = GetNearestMine(t.transform.position);

                    // Set the nearest mine to current tank
                    t.SetNearestMine(mine);

                    mine = GetNearestTeamMine(t.transform.position, village[j].team, true);

                    // Set the nearest mine to current tank
                    t.SetGoodNearestMine(mine);

                    mine = GetNearestTeamMine(t.transform.position, village[j].team, false);

                    // Set the nearest mine to current tank
                    t.SetBadNearestMine(mine);

                    // Think!! 
                    t.Think(dt);

                    // Just adjust tank position when reaching world extents
                    Vector3 pos = t.transform.position;
                    if (pos.x > SceneHalfExtents.x)
                        pos.x -= SceneHalfExtents.x * 2;
                    else if (pos.x < -SceneHalfExtents.x)
                        pos.x += SceneHalfExtents.x * 2;

                    if (pos.z > SceneHalfExtents.z)
                        pos.z -= SceneHalfExtents.z * 2;
                    else if (pos.z < -SceneHalfExtents.z)
                        pos.z += SceneHalfExtents.z * 2;

                    // Set tank position
                    t.transform.position = pos;
                }
            }

            // Check the time to evolve
            accumTime += dt;
            if (accumTime >= GenerationDuration)
            {
                accumTime -= GenerationDuration;
                Epoch();
                break;
            }
        }
    }

    #region Helpers

    Tank CreateTank (Genome genome, NeuralNetwork brain)
    {
        Vector3 position = GetRandomPosInBounds(zoneTanks.bounds);
        GameObject go = Instantiate<GameObject>(TankPrefab, position, GetRandomRot());
        Tank t = go.GetComponent<Tank>();
        t.SetBrain(genome, brain);
        return t;
    }

    void DestroyMines ()
    {
        foreach (Mine mine in mines)
            Destroy(mine.gameObject);

        mines.Clear();
    }

    void DestroyTanks ()
    {
        for (int i = 0; i < village.Count; i++)
        {
            foreach (Tank go in village[i].populationGOs)
                Destroy(go.gameObject);

            village[i].populationGOs.Clear();
            village[i].population.Clear();
            village[i].brains.Clear();
        }
    }

    void CreateMines ()
    {
        // Destroy previous created mines
        DestroyMines();

        for (int i = 0; i < MinesCount; i++)
        {
            Vector3 position = GetRandomPosInBounds(zoneMines.bounds);
            GameObject go = Instantiate(MinePrefab, position, Quaternion.identity);

            Mine mine = go.GetComponent<Mine>();
            Team teamMine = (Team) (i % villageCount);
            go.name = "Mine " + teamMine.ToString();
            mine.SetTeam(teamMine);

            mines.Add(mine);
        }
    }

    public void RelocateMine (GameObject mine)
    {
        mine.transform.position = GetRandomPosInBounds(zoneMines.bounds);
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

    Quaternion GetRandomRot ()
    {
        return Quaternion.identity;
        //return Quaternion.AngleAxis(Random.value * 360.0f, Vector3.up);
    }

    GameObject GetNearestMine (Vector3 pos)
    {
        GameObject nearest = mines[0].gameObject;
        float distance = (pos - nearest.transform.position).sqrMagnitude;

        foreach (Mine mine in mines)
        {
            float newDist = (mine.transform.position - pos).sqrMagnitude;
            if (newDist < distance)
            {
                nearest = mine.gameObject;
                distance = newDist;
            }
        }

        return nearest;
    }

    GameObject GetNearestTeamMine (Vector3 pos, Team team, bool isEqualTeam)
    {
        int indexNearMine = 0;
        float distanceNearMine = float.MaxValue;

        for (int i = 0; i < mines.Count; i++)
        {
            if (isEqualTeam)
            {
                if (mines[i].team != team)
                    continue;
            }
            else
            {
                if (mines[i].team == team)
                    continue;
            }

            float currentDistanceNearMine = (pos - mines[i].transform.position).sqrMagnitude;
            if (currentDistanceNearMine < distanceNearMine)
            {
                indexNearMine = i;
                distanceNearMine = currentDistanceNearMine;
            }
        }

        return mines[indexNearMine].gameObject;
    }
    
    private void OnDrawGizmos ()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(Vector3.zero, SceneHalfExtents * 2);
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(zoneTanks.bounds.center, zoneTanks.bounds.size);
        Gizmos.color = Color.gray;
        Gizmos.DrawWireCube(zoneMines.bounds.center, zoneMines.bounds.size);
        for (int i = 0; i < village.Count; i++)
        {
            village[i].OnDrawGizmos();
        }
    }

    #endregion

}

public enum Team
{
    Blue,
    Red,
    Yellow,
    Green,
    Magenta
}