using System.Collections.Generic;
using UnityEngine;

public class NodeGenerator : MonoBehaviour
{
    public Vector2Int mapSize;
    private Node[] map;
    private Pathfinding pathfinding;

    void Start()
    {
        pathfinding = new Pathfinding();
        NodeUtils.MapSize = mapSize;
        map = new Node[mapSize.x * mapSize.y];
        int ID = 0;
        for (int i = 0; i < mapSize.y; i++)
        {
            for (int j = 0; j < mapSize.x; j++)
            {
                map[ID] = new Node(ID, new Vector2Int(j, i));
                ID++;
            }
        }

        List<Vector2Int> path = pathfinding.GetPath(map, 
            map[NodeUtils.PositionToIndex(new Vector2Int(0, 0))], 
            map[NodeUtils.PositionToIndex(new Vector2Int(8, 3))]);

        for (int i = 0; i < path.Count; i++)
        {
            Debug.Log(path[i]);
        }
    }

    private void OnDrawGizmos()
    {
        if (map == null)
            return;
        Gizmos.color = Color.green;
        GUIStyle style = new GUIStyle() { fontSize = 25  };
        foreach (Node node in map)
        {
            Vector3 worldPosition = new Vector3((float)node.position.x, (float)node.position.y, 0.0f);
            Gizmos.DrawWireSphere(worldPosition, 0.2f);
            //Handles.Label(worldPosition, node.position.ToString(), style);
        }
    }
}
