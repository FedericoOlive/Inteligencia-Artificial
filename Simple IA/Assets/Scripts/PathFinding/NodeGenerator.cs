using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class NodeGenerator : MonoBehaviour
{
    public Vector2Int mapSize;
    [SerializeField] private Node[] map;
    private Pathfinding pathfinding;

    public Vector2Int startPos = new Vector2Int();
    public Vector2Int endPos = new Vector2Int();
    public bool findPath;

    private void Start ()
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
            map[NodeUtils.PositionToIndex(startPos)],
            map[NodeUtils.PositionToIndex(endPos)]);

        for (int i = 0; i < path.Count; i++)
        {
            Debug.Log(path[i]);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected ()
    {
        if (map == null)
            return;
        Gizmos.color = Color.green;
        GUIStyle style = new GUIStyle() {fontSize = 10};
        foreach (Node node in map)
        {

            switch (node.state)
            {
                case Node.NodeState.Open:
                    Gizmos.color = Color.blue;
                    break;
                case Node.NodeState.Closed:
                    Gizmos.color = Color.red;
                    break;
                case Node.NodeState.Ready:
                    Gizmos.color = Color.green;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Vector3 worldPosition = new Vector3((float) node.position.x, (float) node.position.y, 0.0f);
            Handles.Label(worldPosition, node.position.ToString(), style);

            Gizmos.DrawWireSphere(worldPosition, 0.2f);
        }
    }
#endif
}