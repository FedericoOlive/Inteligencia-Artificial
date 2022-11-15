using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class TerrainGenerator : MonoBehaviour
{
    [SerializeField] private List<GameObject> tiles = new List<GameObject>();
    [SerializeField] private GameObject prefabTile;
    [SerializeField] private bool generateTerrain;
    [SerializeField] private int halfHeight = 7;
    [SerializeField] private int halfWidth = 7;
    [SerializeField] private Camera cam;
    public static Vector3 minPos;
    public static Vector3 maxPos;

    [Header("Spawn Settings: ")]
    [SerializeField] private int offsetLimit;
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
        DeInit();

        Vector3 pos = transform.position;

        for (int i = 0; i < halfHeight * 2; i++)
        {
            for (int j = 0; j < halfWidth * 2; j++)
            {
                GameObject tile = Instantiate(prefabTile, pos, Quaternion.identity, transform);
                string space = pos.x > 9 ? "\t" : "";
                tile.name = "Tile: " + pos.x + "\t" + pos.z;
                if (i == halfHeight && j == halfWidth)
                {
                    tile.GetComponentInChildren<SpriteRenderer>().color = Color.black;
                }

                tiles.Add(tile);
                pos.x += 1;

                SetSpawnPoint(i, j, tile);
            }

            pos.x = transform.position.z;
            pos.z += 1;
        }

        cam.transform.position = new Vector3(halfHeight + 0.5f, 10, halfWidth + 0.5f);
    }

    private void SetSpawnPoint (int i, int j, GameObject tile)
    {
        if (i == offsetLimit)
        {
            if (j > offsetLimit && j < halfWidth * 2 - offsetLimit - 1)
            {
                tile.GetComponentInChildren<SpriteRenderer>().color = Color.blue;
                spawnPointsDown.Add(tile.transform);
            }
        }
        else if (i == halfHeight * 2 - offsetLimit - 1)
        {
            if (j > offsetLimit && j < halfWidth * 2 - offsetLimit - 1)
            {
                tile.GetComponentInChildren<SpriteRenderer>().color = Color.red;
                spawnPointsUp.Add(tile.transform);
            }
        }

        if (j == offsetLimit)
        {
            if (i > offsetLimit && i < halfWidth * 2 - offsetLimit - 1)
            {
                tile.GetComponentInChildren<SpriteRenderer>().color = Color.green;
                spawnPointsLeft.Add(tile.transform);
            }
        }
        else if (j == halfHeight * 2 - offsetLimit - 1)
        {
            if (i > offsetLimit && i < halfWidth * 2 - offsetLimit - 1)
            {
                tile.GetComponentInChildren<SpriteRenderer>().color = Color.magenta;
                spawnPointsRight.Add(tile.transform);
            }
        }
    }

    public void DeInit ()
    {
        generateTerrain = false;

        if (!Application.isPlaying)
            for (int i = 0; i < tiles.Count; i++)
                DestroyImmediate(tiles[i].gameObject);
        else
            for (int i = 0; i < tiles.Count; i++)
                Destroy(tiles[i].gameObject);

        tiles.Clear();

        maxPos = new Vector3(halfWidth, 0, halfHeight);
        minPos = maxPos * -1;
    }

    public static bool IsPositionInsideBounds (Vector3 pos)
    {
        if (pos.x > minPos.x && pos.x < maxPos.x) // Ancho
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