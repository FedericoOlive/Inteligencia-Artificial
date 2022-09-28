using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TerrainSettings
{
    public static int GetWeightByTerrainType (TerrainCellType cellType)
    {
        int weight = 0;
        switch (cellType)
        {
            case TerrainCellType.Grass:
                weight = 1;
                break;
            case TerrainCellType.Sand:
                weight = 5;
                break;
            case TerrainCellType.Rock:
                weight = 10;
                break;
            default:
                break;
        }

        return weight;
    }

    public static float GetSpeedInTerrain (Vector3 pos)
    {
        Vector3Int position = NodeUtils.GetVec3IntFromVector3(pos);
        float weight = NodeGenerator.GetMap[NodeUtils.PositionToIndex(position)].weight;
        return 1 / weight;
    }
}

public enum TerrainCellType
{
    Grass,
    Sand,
    Rock
}