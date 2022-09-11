using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class CustomTerrain : MonoBehaviour
{
    [SerializeField] private GameObject cube;
    [SerializeField] private int spawnColumns;
    [SerializeField] private int spawnRows;

    [SerializeField] private bool spawnButton;

    [SerializeField] private List<GameObject> cubes = new List<GameObject>();
    [SerializeField] private Material[] materials;
    private int[] percentagesCellType = new[] {50, 70, 90, 100};

    private void Update ()
    {
        if (spawnButton)
        {
            spawnButton = false;
            ResetHexagons();

            Vector3 pos = Vector3.zero;
            Vector3 posInit = transform.position;
            for (int i = 0; i < spawnColumns; i++)
            {
                for (int j = 0; j < spawnRows; j++)
                {
                    GameObject newCube = Instantiate(cube, pos + posInit, Quaternion.identity, transform);
                    pos.x += 1f;
                    cubes.Add(newCube);

                    Cell cell = newCube.GetComponent<Cell>();

                    cell.cellType = GetCellTypeByRandom();
                    cell.material = materials[(int) cell.cellType];
                }

                pos.x = 0;
                pos.z += 1f;
            }
        }
    }

    private void ResetHexagons ()
    {
        foreach (GameObject hexagon in cubes)
        {
            DestroyImmediate(hexagon.gameObject);
        }

        cubes.Clear();
    }

    private CellType GetCellTypeByRandom ()
    {
        int random = Random.Range(0, 100);

        for (int i = 0; i < percentagesCellType.Length; i++)
        {
            if (random < percentagesCellType[i])
            {
                return (CellType) i;
            }
        }

        return CellType.Grass;
    }

    //public static Vector3 GetAvailablePosition (Vector2 min, Vector2 max, Vector3 transformPosition)
    //{
    //    CellType cellType = CellType.Water;
    //    Vector3 randomPos;
    //
    //    do
    //    {
    //        randomPos = new Vector3(Random.Range((int) min.x, (int) min.y), 0, Random.Range((int) max.x, (int) max.y));
    //        randomPos += new Vector3(100, 0, 100);
    //
    //        if (Physics.Raycast(randomPos, Vector3.down * 10, out RaycastHit hit))
    //        {
    //            Cell cell = hit.transform.GetComponent<Cell>();
    //            if (cell)
    //            {
    //                cellType = cell.cellType;
    //            }
    //        }
    //
    //    } while (cellType == CellType.Water);
    //
    //    return randomPos;
    //}
}

public enum CellType
{
    Grass,
    Sand,
    Water,
    Mountain,
    Last
}