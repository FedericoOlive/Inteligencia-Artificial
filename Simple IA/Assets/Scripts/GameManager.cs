using System.Collections.Generic;
using UnityEngine;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class GameManager : MonoBehaviourSingleton<GameManager>
{
    private bool isrunning;
    [SerializeField] private bool initWorld;
    [SerializeField] private bool clearWorld;
    [SerializeField] private TerrainGenerator terrainGenerator;
    [SerializeField] private FoodManager foodManager;
    [SerializeField] private List<PopulationManager> populationManager = new List<PopulationManager>();

    [SerializeField] private LevelSettings levelSettings;
    [SerializeField] private DataPopulation dataPopulation;
    public DataAI dataAI;
    public TextAsset dataString;
    
    public float accumTime;
    public float accumRounds;
    
    private void Update ()
    {
        if (!isrunning)
            return;
        if (initWorld)
        {
            initWorld = false;
            Init();
        }
        else if (clearWorld)
        {
            clearWorld = false;
            DeInit();
        }
    }

    private void FixedUpdate ()
    {
        if (!isrunning)
            return;

        float deltaTime = Time.fixedDeltaTime;
        accumRounds++;
        accumTime += deltaTime;

        for (int i = 0; i < populationManager.Count; i++)
        {
            populationManager[i].FixedUpdateForGameManager(deltaTime);
        }


        CheckVillagerToEatAndFight(); // Tengo que unificar estos 2 comportamientos porque sino queda 1 de cada uno intentando comer una comida (Para el 2do no va a existir)
        CheckVillagerToEatOrRun();
        EatVillagersAlives();

        RemoveAllKilleds();

        CheckForEpoch();
    }
    
    private void CheckVillagerToEatAndFight()
    {
        List<Villager> villagers = GetVillagersByState(StateAttack.FightAndEat);
        List<EatGroup> fightGroups = new List<EatGroup>();

        // Agarro los Grupos que están en la misma posicion y sobre la comida y que hayan decidido pelear por ella
        for (int i = 0; i < villagers.Count; i++)
        {
            List<Villager> villagersEqualPositions = new List<Villager>();
            for (int j = i + 1; j < villagers.Count; j++)
            {
                if (villagers[i].transform.position == villagers[j].transform.position)
                {
                    villagersEqualPositions.Add(villagers[i]);
                    villagersEqualPositions.Add(villagers[j]);
                }
            }

            if (villagersEqualPositions.Count > 1)
            {
                EatGroup eatGroup = new EatGroup();
                eatGroup.villagers = villagersEqualPositions;
                fightGroups.Add(eatGroup);
            }
        }

        // Pelean todos los que están en la comida
        for (int i = 0; i < fightGroups.Count; i++)
        {
            fightGroups[i].Combat();
        }

        // Mueren a los que la vida le llega a 0
        for (int i = 0; i < villagers.Count; i++)
        {
            if (villagers[i].life < 1)
                villagers[i].Kill();
        }
    }

    private void CheckVillagerToEatOrRun()
    {
        List<Villager> villagers = GetVillagersByState(StateAttack.EatOrRun);
        List<EatGroup> fightGroups = new List<EatGroup>();

        // Agarro los Grupos que están en la misma posicion y sobre la comida y que hayan decidido pelear por ella
        for (int i = 0; i < villagers.Count; i++)
        {
            List<Villager> villagersEqualPositions = new List<Villager>();
            for (int j = i + 1; j < villagers.Count; j++)
            {
                if (villagers[i].transform.position == villagers[j].transform.position)
                {
                    villagersEqualPositions.Add(villagers[i]);
                    villagersEqualPositions.Add(villagers[j]);
                }
            }

            if (villagersEqualPositions.Count > 1)
            {
                EatGroup eatGroup = new EatGroup();
                eatGroup.villagers = villagersEqualPositions;
                fightGroups.Add(eatGroup);
            }
        }

        // Se decide quien se queda con la comida
        for (int i = 0; i < fightGroups.Count; i++)
        {
            fightGroups[i].KeepFood();
        }
    }

    private void EatVillagersAlives()
    {
        for (int i = 0; i < populationManager.Count; i++)
        {
            for (int j = 0; j < populationManager[i].village.populationGOs.Count; j++)
            {
                Villager villager = populationManager[i].village.populationGOs[j];
                if (villager.isAlive)
                {
                    StateAttack state = villager.stateAttack;
                    if (state == StateAttack.FightAndEat)
                    {
                        Food food = foodManager.GetFoodInPosition(villager.transform.position);
                        if (food)
                        {
                            villager.TakeFood(food);
                        }
                        else
                        {
                            Debug.LogError("No existe esta Food en esta posicion: " + villager.transform.position);
                            Debug.LogError("Para este Villager: ", villager);
                            Debug.Break();
                        }
                    }
                }
            }
        }

        for (int i = 0; i < populationManager.Count; i++)
        {
            for (int j = 0; j < populationManager[i].village.populationGOs.Count; j++)
            {
                Villager villager = populationManager[i].village.populationGOs[j];
                if (villager.isAlive)
                {
                    StateAttack state = villager.stateAttack;
                    if (state == StateAttack.EatOrRun)
                    {
                        Food food = foodManager.GetFoodInPosition(villager.transform.position);
                        if (food)
                        {
                            villager.TakeFood(food);
                        }
                        else
                        {
                            villager.GoBackPosition();
                        }
                    }
                }
            }
        }
    }

    public void CheckForEpoch()
    {
        switch (levelSettings.generationEndType)
        {
            case GenerationEndType.Manual:
                break;
            case GenerationEndType.Time:
                if (accumTime > levelSettings.timeGenerationDuration)
                {
                    accumRounds = 0;
                    accumTime = 0;
                    EpochAll();
                }
                break;
            case GenerationEndType.Rounds:
                if (accumRounds > levelSettings.roundsGenerationDuration)
                {
                    accumRounds = 0;
                    accumTime = 0;
                    EpochAll();
                }
                break;
        }
    }

    void EpochAll ()
    {
        // Kill survivals 3 generations
        for (int i = 0; i < populationManager.Count; i++)
        {
            for (int j = 0; j < populationManager[i].village.populationGOs.Count; j++)
            {
                populationManager[i].village.populationGOs[j].generationsAlive--;

                if (populationManager[i].village.populationGOs[j].generationsAlive < 1)
                {
                    populationManager[i].village.populationGOs[j].Kill();
                }
            }
        }

        RemoveAllKilleds();
        
        List<int> indexPopDeads = new List<int>();
        for (int i = 0; i < populationManager.Count; i++)
        {
            populationManager[i].Epoch();
            if (populationManager[i].village.populationGOs.Count < 1)
                indexPopDeads.Add(i);
        }

        if (indexPopDeads.Count == levelSettings.maxCivilizations)
        {
            Debug.Log("Se reinicia la Simulación");
            // Murieron Todas las civs
            for (int i = 0; i < populationManager.Count; i++)
            {
                populationManager[i].StartSimulation();
            }
        }
        else if (indexPopDeads.Count > 0)
        {
            int indexPopAlive = GetIndexPopAlive(indexPopDeads);

            for (int i = 0; i < indexPopDeads.Count; i++)
            {
                //populationManager[indexPopDeads[i]].CopyPopulation(populationManager[indexPopAlive]);
            }
        }
    }

    int GetIndexPopAlive (List<int> indexPopDeads)
    {
        for (int i = 0; i < indexPopDeads.Count; i++)
        {
            bool isOk = true;
            for (int j = 0; j < indexPopDeads.Count; j++)
            {
                if (indexPopDeads[i] == j)
                {
                    isOk = false;
                    break;
                }
            }

            if (isOk)
                return i;
        }

        return -1;
    }

    void RemoveAllKilleds ()
    {
        for (int i = 0; i < populationManager.Count; i++)
        {
            List<int> villagers = new List<int>();

            for (int j = populationManager[i].village.populationGOs.Count - 1; j >= 0; j--)
            {
                Villager villager = populationManager[i].village.populationGOs[j];

                if (!villager.isAlive)
                {
                    Destroy(populationManager[i].village.populationGOs[j].gameObject);
                    populationManager[i].village.populationGOs.RemoveAt(j);
                    populationManager[i].village.population.RemoveAt(j);
                    populationManager[i].village.brains.RemoveAt(j);
                }
            }
        }
    }
    

    public void Init ()
    {
        if (Application.isPlaying)
            isrunning = true;
        terrainGenerator.Init();
        foodManager.Init();

        int maxCivAmount = Mathf.Min(levelSettings.maxCivilizations, populationManager.Count);
        for (int i = 0; i < maxCivAmount; i++)
            populationManager[i].Init();
    }

    private void DeInit ()
    {
        terrainGenerator.DeInit();
        foodManager.DeInit();

        for (int i = 0; i < populationManager.Count; i++)
            populationManager[i].DeInit();
    }

    public Food GetNearFood (Vector3 pos)
    {
        return foodManager.GetNearFood(pos);
    }

    public void GetEnemyNearFood (Food food, Team myTeam)
    {

    }

    private List<Villager> GetVillagersByState (StateAttack state)
    {
        List<Villager> villagers = new List<Villager>();

        for (int i = 0; i < populationManager.Count; i++)
        {
            for (int j = 0; j < populationManager[i].village.populationGOs.Count; j++)
            {
                if (populationManager[i].village.populationGOs[j].stateAttack == state)
                {
                    villagers.Add(populationManager[i].village.populationGOs[j]);
                }
            }
        }

        return villagers;
    }


    public void LoadData()
    {
        string path;

#if UNITY_EDITOR
        path = EditorUtility.OpenFilePanel("Select Brain Data", "", "json");
#endif
        DataAI dataAi = null;
        string data;

        if (string.IsNullOrEmpty(path))
        {
            if (dataString == null)
                data = string.Empty;
            else
                data = dataString.text;
        }
        else
        {
            data = File.ReadAllText(path);
        }
        dataAi = JsonUtility.FromJson<DataAI>(data);

        if (dataAi == null) return;

        dataPopulation.eliteCount = dataAi.eliteCount;
        dataPopulation.mutationChance = dataAi.mutationChance;
        dataPopulation.mutationRate = dataAi.mutationRate;

        dataPopulation.inputsCount = dataAi.inputsCount;
        dataPopulation.hiddenLayers = dataAi.hiddenLayers;
        dataPopulation.outputsCount = dataAi.outputsCount;
        dataPopulation.neuronsCountPerHL = dataAi.neuronsCountPerHL;
        dataPopulation.bias = dataAi.bias;
        dataPopulation.p = dataAi.p;

        Init();

        for (int i = 0; i < populationManager.Count; i++)
        {
            populationManager[i].GenerateInitialPopulation(dataAi.genomes);
        }
    }

    public void SaveData (int indexPopulation)
    {
        string path = null;

#if UNITY_EDITOR
        path = EditorUtility.SaveFilePanel("Save Brain Data", "", "brain_data.json", "json");
#endif

        if (string.IsNullOrEmpty(path)) return;

        DataAI data = new DataAI();

        data.genomes = populationManager[indexPopulation].village.population;

        data.eliteCount = dataPopulation.eliteCount;
        data.mutationChance = dataPopulation.mutationChance;
        data.mutationRate = dataPopulation.mutationRate;

        data.inputsCount = dataPopulation.inputsCount;
        data.hiddenLayers = dataPopulation.hiddenLayers;
        data.outputsCount = dataPopulation.outputsCount;
        data.neuronsCountPerHL = dataPopulation.neuronsCountPerHL;
        data.bias = dataPopulation.bias;
        data.p = dataPopulation.p;

        string dataJson = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, dataJson);
    }

    public List<PopulationManager> GetPopulations () => populationManager;
}

public class EatGroup
{
    public List<Villager> villagers = new List<Villager>();

    public void Combat ()
    {
        bool pacify = true;
        Team team = villagers[0].team;
        for (int i = 0; i < villagers.Count; i++)
        {
            if (team != villagers[i].team)
            {
                pacify = false; // Agregar Break
                break;
            }
        }

        if (pacify)
        {
            int indexEat = GetIndexMaxLife();

            for (int i = 0; i < villagers.Count; i++)
            {
                if (i == indexEat)
                    continue;
                villagers[i].GoBackPosition();
                villagers[i].stateAttack = StateAttack.None;
            }
        }
        else
        {
            int indexMaxLifeA = -1;
            int indexMaxLifeB = -1;
            int maxLifeA = 0;
            int maxLifeB = 0;

            for (int i = 0; i < villagers.Count; i++)
            {
                if (villagers[i].life > maxLifeA)
                {
                    maxLifeA = villagers[i].life;
                    indexMaxLifeA = i;
                }
            }

            for (int i = 0; i < villagers.Count; i++)
            {
                if (i == indexMaxLifeA)
                    continue;

                if (villagers[i].life > maxLifeB)
                {
                    maxLifeB = villagers[i].life;
                    indexMaxLifeB = i;
                }
            }

            int indexWinner = (maxLifeA > maxLifeB) ? indexMaxLifeA : indexMaxLifeB;
            int indexLoser = (maxLifeA > maxLifeB) ? indexMaxLifeB : indexMaxLifeA;

            for (int i = 0; i < villagers.Count; i++)
            {
                if (i == indexWinner)
                {
                    villagers[indexWinner].life -= maxLifeB;
                }
                else
                {
                    villagers[i].stateAttack = StateAttack.None;
                    villagers[i].Kill();
                }
            }
        }
    }

    public void KeepFood ()
    {
        int indexEat = GetIndexMaxLife();

        for (int i = 0; i < villagers.Count; i++)
        {
            if (i == indexEat)
                continue;
            villagers[i].GoBackPosition();
            villagers[i].stateAttack = StateAttack.None;
        }
    }

    private int GetIndexMaxLife ()
    {
        int indexEat = 0;
        int life = 0;

        for (int i = 0; i < villagers.Count; i++)
        {
            if (villagers[i].life > life)
            {
                indexEat = 0;
                life = villagers[i].life;
            }
        }

        return indexEat;
    }
}

public class KeepGeneration
{
    public List<Villager> populationGOs = new List<Villager>();
    public List<Genome> population = new List<Genome>();
    public List<NeuralNetwork> brains = new List<NeuralNetwork>();
}