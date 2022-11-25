using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class TerrainGenerator : MonoBehaviour
{
    [SerializeField] private LevelSettings levelSettings;
    [SerializeField] private List<GameObject> tiles = new List<GameObject>();
    [SerializeField] private GameObject prefabTile;
    [SerializeField] private bool generateTerrain;
    [SerializeField] private Camera cam;
    public static Vector3 minPos;
    public static Vector3 maxPos;

    [Header("Spawn Settings: ")]
    private static List<Transform> spawnPointsUp = new List<Transform>();
    private static List<Transform> spawnPointsRight = new List<Transform>();
    private static List<Transform> spawnPointsDown = new List<Transform>();
    private static List<Transform> spawnPointsLeft = new List<Transform>();

    private void Update ()
    {
        if (generateTerrain)
        {
            Init();
        }
    }

    public void Init ()
    {
        maxPos = new Vector3(levelSettings.halfWidth * 2, 0, levelSettings.halfHeight * 2 + 1);
        minPos = new Vector3(1, 0, 0);

        DeInit();

        Vector3 pos = transform.position;

        for (int i = 0; i < levelSettings.halfHeight * 2; i++)
        {
            for (int j = 0; j < levelSettings.halfWidth * 2; j++)
            {
                GameObject tile = Instantiate(prefabTile, pos, Quaternion.identity, transform);
                tile.name = "Tile: " + pos.x + "\t" + pos.z;
                if (i == levelSettings.halfHeight && j == levelSettings.halfWidth)
                {
                    tile.GetComponentInChildren<SpriteRenderer>().color = Color.black;
                    levelSettings.autoRePositionFoodCenter = tile.transform.position;
                }

                tiles.Add(tile);
                pos.x += 1;

                SetSpawnPoint(i, j, tile);
            }

            pos.x = transform.position.z;
            pos.z += 1;
        }

        spawnPointsLeft.Reverse();
        spawnPointsUp.Reverse();

        cam.transform.position = new Vector3(levelSettings.halfHeight + 0.5f, 10, levelSettings.halfWidth + 0.5f);
    }

    private void SetSpawnPoint (int i, int j, GameObject tile)
    {
        if (i == levelSettings.offsetLimit)
        {
            if (j > levelSettings.offsetLimit && j < levelSettings.halfWidth * 2 - levelSettings.offsetLimit - 1)
            {
                tile.GetComponentInChildren<SpriteRenderer>().color = Color.blue;
                spawnPointsDown.Add(tile.transform);
            }
        }
        else if (i == levelSettings.halfHeight * 2 - levelSettings.offsetLimit - 1)
        {
            if (j > levelSettings.offsetLimit && j < levelSettings.halfWidth * 2 - levelSettings.offsetLimit - 1)
            {
                tile.GetComponentInChildren<SpriteRenderer>().color = Color.red;
                spawnPointsUp.Add(tile.transform);
            }
        }

        if (j == levelSettings.offsetLimit)
        {
            if (i > levelSettings.offsetLimit && i < levelSettings.halfWidth * 2 - levelSettings.offsetLimit - 1)
            {
                tile.GetComponentInChildren<SpriteRenderer>().color = Color.green;
                spawnPointsLeft.Add(tile.transform);
            }
        }
        else if (j == levelSettings.halfHeight * 2 - levelSettings.offsetLimit - 1)
        {
            if (i > levelSettings.offsetLimit && i < levelSettings.halfWidth * 2 - levelSettings.offsetLimit - 1)
            {
                tile.GetComponentInChildren<SpriteRenderer>().color = Color.magenta;
                spawnPointsRight.Add(tile.transform);
            }
        }
    }

    public void DeInit ()
    {
        spawnPointsUp.Clear();
        spawnPointsRight.Clear();
        spawnPointsDown.Clear();
        spawnPointsLeft.Clear();

        generateTerrain = false;

        if (!Application.isPlaying)
            for (int i = 0; i < tiles.Count; i++)
                DestroyImmediate(tiles[i].gameObject);
        else
            for (int i = 0; i < tiles.Count; i++)
                Destroy(tiles[i].gameObject);

        tiles.Clear();
    }

    public static bool IsPositionInsideBounds (Vector3 pos)
    {
        //if (pos.x > minPos.x && pos.x < maxPos.x) // Ancho (No se chequea en ancho porque debe spawnear del otro lado)
            if (pos.z > minPos.z && pos.z < maxPos.z) // Alto
                return true;

        return false;
    }

    public static List<Transform> GetSpawnPoints (int spawnPointType)
    {
        return GetSpawnPoints((SpawnPointType) spawnPointType);
    }

    public static List<Transform> GetSpawnPoints (SpawnPointType spawnPointType)
    {
        switch (spawnPointType)
        {
            case SpawnPointType.Up:
                return spawnPointsUp;
            case SpawnPointType.Right:
                return spawnPointsRight;
            case SpawnPointType.Down:
                return spawnPointsDown;
            case SpawnPointType.Left:
                return spawnPointsLeft;
        }

        return null;
    }
}

public enum SpawnPointType
{
    Up,
    Right,
    Down,
    Left
}