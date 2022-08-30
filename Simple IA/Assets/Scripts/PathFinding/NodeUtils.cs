using System.Collections.Generic;
using UnityEngine;

public static class NodeUtils
{
    public static Vector3Int MapSize;

    public static List<int> GetAdjacentsNodeIDs (Vector3Int position)
    {
        List<int> IDs = new List<int>();
        IDs.Add(PositionToIndex(new Vector3Int(position.x + 1, 0, position.z)));
        IDs.Add(PositionToIndex(new Vector3Int(position.x,     0, position.z - 1)));
        IDs.Add(PositionToIndex(new Vector3Int(position.x - 1, 0, position.z)));
        IDs.Add(PositionToIndex(new Vector3Int(position.x,     0, position.z + 1)));
        return IDs;
    }

    public static int PositionToIndex (Vector3Int position)
    {
        if (position.x < 0 || position.x >= MapSize.x || position.z < 0 || position.z >= MapSize.z)
            return -1;
        int aux = position.z * MapSize.x + position.x;
        return aux;
    }
}