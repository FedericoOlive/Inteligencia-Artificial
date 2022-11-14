using System;
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

    private void Update ()
    {
        if (generateTerrain)
        {
            ResetTiles();

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
                        tile.GetComponentInChildren<SpriteRenderer>().color = Color.red;
                    }

                    tiles.Add(tile);
                    pos.x += 1;
                }

                pos.x = transform.position.z;
                pos.z += 1;
            }

            cam.transform.position = new Vector3(halfHeight + 0.5f, 10, halfWidth + 0.5f);
        }
    }

    private void ResetTiles ()
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
        if (pos.x > minPos.x && pos.x < maxPos.x)       // Ancho
            if (pos.z > minPos.z && pos.z < maxPos.z)   // Alto
                return true;

        return false;
    }
}