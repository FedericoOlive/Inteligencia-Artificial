using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class Terrain : MonoBehaviour
{
    [SerializeField] private GameObject cube;
    [SerializeField] private int spawnColumns;
    [SerializeField] private int spawnRows;

    [SerializeField] private bool spawnButton;

    [SerializeField] private List<GameObject> cubes = new List<GameObject>();

    void Update ()
    {
        if (spawnButton)
        {
            spawnButton = false;
            ResetHexagons();

            Vector3 pos = Vector3.zero;
            for (int i = 0; i < spawnColumns; i++)
            {
                for (int j = 0; j < spawnRows; j++)
                {
                    GameObject newCube = Instantiate(cube, pos, Quaternion.identity, transform);
                    pos.x += 1.1f;
                    cubes.Add(newCube);

                    Cell cell = newCube.GetComponent<Cell>();
                    //cell.Init((CellType)Random.Range(0, (int)CellType.Last));
                }

                pos.x = 0;
                pos.y += 1.1f;
            }
        }
    }

    void ResetHexagons ()
    {
        foreach (GameObject hexagon in cubes)
        {
            DestroyImmediate(hexagon.gameObject);
        }

        cubes.Clear();
    }
}