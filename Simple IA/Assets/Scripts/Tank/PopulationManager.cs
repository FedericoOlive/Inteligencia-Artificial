using UnityEngine;
using System.Collections.Generic;

public class PopulationManager : MonoBehaviour
{
    public static event System.Action OnEpoch;
    public Collider zoneTanks;

    public GameObject TankPrefab;

    public int villageCount = 2;
    public int PopulationCount = 10;
    public int FoodsCount = 50;
    
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

    List<Food> Foods = new List<Food>();

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
        genAlg = new GeneticAlgorithm(EliteCount, MutationChance, MutationRate);

        GenerateInitialPopulation();

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

        // Destroy all Foods
        DestroyFoods();
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
            float currentFitness = village[i].bestFitness;
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
                    village[i].ResetFitness();
                }

                continue;
            }

            // Increment generation counter
            village[i].generation++;
            
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
    
    void FixedUpdate ()
    {
        if (!isRunning)
            return;

        for (int i = 0; i < village.Count; i++)
            village[i].UpdateFitness();

        float dt = Time.fixedDeltaTime;

        for (int i = 0; i < Mathf.Clamp((float) (IterationCount / 100.0f) * 50, 1, 50); i++)
        {
            for (int j = 0; j < village.Count; j++)
            {
                foreach (Villager tank in village[j].populationGOs)
                {
                    Food food = GetNearestFood(tank.transform.position);
                    tank.SetNearestFood(food);
                    tank.Think(dt);
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

    Villager CreateTank (Genome genome, NeuralNetwork brain)
    {
        Vector3 position = GetRandomPosInBounds(zoneTanks.bounds);
        GameObject go = Instantiate<GameObject>(TankPrefab, position, GetRandomRot());
        Villager t = go.GetComponent<Villager>();
        t.SetBrain(genome, brain);
        return t;
    }

    void DestroyFoods ()
    {
        foreach (Food food in Foods)
            Destroy(food.gameObject);

        Foods.Clear();
    }

    void DestroyTanks ()
    {
        for (int i = 0; i < village.Count; i++)
        {
            foreach (Villager go in village[i].populationGOs)
                Destroy(go.gameObject);

            village[i].populationGOs.Clear();
            village[i].population.Clear();
            village[i].brains.Clear();
        }
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

    Food GetNearestFood (Vector3 pos)
    {
        Food nearest = Foods[0];
        float distance = (pos - nearest.transform.position).sqrMagnitude;

        foreach (Food food in Foods)
        {
            float newDist = (food.transform.position - pos).sqrMagnitude;
            if (newDist < distance)
            {
                nearest = food;
                distance = newDist;
            }
        }

        return nearest;
    }

    Food GetNearestTeamFood (Vector3 pos, Team team, bool isEqualTeam)
    {
        int indexNearFood = 0;
        float distanceNearFood = float.MaxValue;

        for (int i = 0; i < Foods.Count; i++)
        {
            if (isEqualTeam)
            {
                if (Foods[i].team != team)
                    continue;
            }
            else
            {
                if (Foods[i].team == team)
                    continue;
            }

            float currentDistanceNearFood = (pos - Foods[i].transform.position).sqrMagnitude;
            if (currentDistanceNearFood < distanceNearFood)
            {
                indexNearFood = i;
                distanceNearFood = currentDistanceNearFood;
            }
        }

        return Foods[indexNearFood];
    }

    private void OnDrawGizmos ()
    {
        for (int i = 0; i < village.Count; i++)
        {
            village[i].OnDrawGizmos();
        }
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