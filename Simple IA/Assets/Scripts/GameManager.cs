using UnityEngine;

[ExecuteAlways]
public class GameManager : MonoBehaviourSingleton<GameManager>
{
    [SerializeField] private bool initWorld;
    [SerializeField] private bool clearWorld;
    [SerializeField] private TerrainGenerator terrainGenerator;
    [SerializeField] private FoodManager foodManager;
    [SerializeField] private PopulationManager populationManager;

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

    void Init ()
    {
        terrainGenerator.Init();
        foodManager.Init();

        populationManager.Init();
    }

    void DeInit ()
    {
        terrainGenerator.DeInit();
        foodManager.DeInit();

        populationManager.DeInit();
    }

    public Food GetNearFood (Vector3 pos)
    {
        return foodManager.GetNearFood(pos);
    }
}