using UnityEngine;

[CreateAssetMenu(fileName = "Level Settings", menuName = "Settings/Level")]
public class LevelSettings : ScriptableObject
{
    private const int spaceHeaders = 10;

    [Header("Terrain: ")]
    public int halfHeight = 20;
    public int halfWidth = 20;
    public int offsetLimit = 1;

    [Space(spaceHeaders), Header("Food: ")]
    public Vector3 autoRePositionFoodCenter;
    public int size = 20;
    public int maxFood = 20;

    [Space(spaceHeaders), Header("Population: ")] 
    public int maxCivilizations = 4;
    public int maxGeneationsAlive = 3;
    public GenerationEndType generationEndType = GenerationEndType.Manual;
    public float timeGenerationDuration = 20;
    public int roundsGenerationDuration = 20;
}