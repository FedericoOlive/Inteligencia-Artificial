using System.Collections.Generic;
using UnityEngine;

public static class NodeUtils
{
    public static Vector3Int MapSize;

    public static List<int> GetAdjacentsNodeIDs (Vector3Int position)
    {
        List<int> IDs = new List<int>();

        int idNode = PositionToIndex(new Vector3Int(position.x + 1, 0, position.z));
        if (idNode != -1)
            IDs.Add(idNode);
        idNode = PositionToIndex(new Vector3Int(position.x, 0, position.z - 1));
        if (idNode != -1)
            IDs.Add(idNode);
        idNode = PositionToIndex(new Vector3Int(position.x - 1, 0, position.z));
        if (idNode != -1)
            IDs.Add(idNode);
        idNode = PositionToIndex(new Vector3Int(position.x, 0, position.z + 1));
        if (idNode != -1)
            IDs.Add(idNode);
        return IDs;
    }

    public static int PositionToIndex (Vector3Int position)
    {
        if (position.x < 0 || position.x >= MapSize.x || position.z < 0 || position.z >= MapSize.z)
            return -1;
        int aux = position.z * MapSize.x + position.x;
        return aux;
    }

    public static float GetDistanceXZ(Vector3 vec3One, Vector3 vec3Two)
    {
        Vector2 pos1 = new Vector2(vec3One.x, vec3One.z);
        Vector2 pos2 = new Vector2(vec3Two.x, vec3Two.z);
        return Vector2.Distance(pos1, pos2);
    }
}