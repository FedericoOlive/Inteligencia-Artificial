using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class GameManager : MonoBehaviourSingleton<GameManager>
{
    [SerializeField] private bool initWorld;
    [SerializeField] private bool clearWorld;
    [SerializeField] private TerrainGenerator terrainGenerator;
    [SerializeField] private FoodManager foodManager;
    [SerializeField] private List<PopulationManager> populationManager = new List<PopulationManager>();

    [SerializeField] private LevelSettings levelSettings;

    private void Start ()
    {
        Init();
    }

    private void Update ()
    {
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
        float deltaTime = Time.fixedDeltaTime;
        for (int i = 0; i < populationManager.Count; i++)
        {
            populationManager[i].FixedUpdateForGameManager(deltaTime);
        }

        CheckVillagerToEatOrRun();
        CheckVillagerToEatAndFight();
        EatVillagersAlives();


        for (int i = 0; i < populationManager.Count; i++)
        {
            populationManager[i].CheckForEpoch();
        }
    }
    
    private void Init ()
    {
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

    private void CheckVillagerToEatAndFight ()
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

        // Mato a los que la vida le llega a 0
        for (int i = 0; i < villagers.Count; i++)
        {
            if (villagers[i].life < 1)
                villagers[i].Kill();
        }
    }

    private void CheckVillagerToEatOrRun ()
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

        // Pelean todos los que están en la comida
        for (int i = 0; i < fightGroups.Count; i++)
        {
            fightGroups[i].KeepFood();
        }
    }

    private void EatVillagersAlives ()
    {
        for (int i = 0; i < populationManager.Count; i++)
        {
            for (int j = 0; j < populationManager[i].village.populationGOs.Count; j++)
            {
                Villager villager = populationManager[i].village.populationGOs[j];
                if (villager.life > 0)
                {
                    StateAttack state = villager.stateAttack;
                    if (state == StateAttack.EatOrRun || state == StateAttack.FightAndEat)
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
                pacify = false;
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
                    villagers[indexWinner].life -= villagers[indexLoser].life;
                }
                else
                {
                    villagers[i].stateAttack = StateAttack.None;
                    villagers[i].life = 0;
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