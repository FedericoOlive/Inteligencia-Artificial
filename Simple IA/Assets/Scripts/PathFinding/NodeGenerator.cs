using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class NodeGenerator : MonoBehaviour
{
    public Vector3Int mapSize;
    private static Node[] map;
    private Pathfinding pathfinding;

    public Vector3Int startPos = new Vector3Int();
    public Vector3Int endPos = new Vector3Int();

    [SerializeField] private bool showLabel;
    [SerializeField] private int sizeLabel = 10;
    [SerializeField] private Vector3 offsetLabel = new Vector3(-0.45f, -0.45f, 0);
    [SerializeField] private float alphaColor = 0.5f;
    private List<Vector3Int> path = new List<Vector3Int>();

    private void Start ()
    {
        pathfinding = new Pathfinding();
        NodeUtils.MapSize = mapSize;
        map = new Node[mapSize.x * mapSize.z];
        int ID = 0;
        for (int i = 0; i < mapSize.z; i++)
        {
            for (int j = 0; j < mapSize.x; j++)
            {
                map[ID] = new Node(ID, new Vector3Int(j,0, i));
                ID++;
            }
        }

        path = pathfinding.GetPath(map, startPos, endPos);
    }

    public void GetPath (Vector3Int origin, Vector3Int end) => pathfinding.GetPath(map, map[NodeUtils.PositionToIndex(origin)], map[NodeUtils.PositionToIndex(end)]);
    
    public static Node[] GetMap () => map;


#if UNITY_EDITOR
    private void OnDrawGizmos ()
    {
        if (map == null)
            return;
        Color newColor = Color.black;
        GUIStyle style = new GUIStyle() { fontSize = sizeLabel };
        //
        //foreach (Node node in map)
        //{
        //    switch (node.viewerId)
        //    {
        //        case -1:
        //            newColor = Color.black;
        //            break;
        //        case 0:
        //            newColor = Color.red;
        //            break;
        //        case 1:
        //            newColor = Color.blue;
        //            break;
        //        case 2:
        //            newColor = Color.green;
        //            break;
        //        case 3:
        //            newColor = Color.yellow;
        //            break;
        //        default:
        //            newColor = Color.magenta;
        //            break;
        //    }
        //
        //    newColor.a = alphaColor;
        //    Gizmos.color = newColor;
        //    Gizmos.DrawCube(node.position, new Vector3(1, 0, 1));
        //    Vector3Int pos = node.position;
        //    if (showLabel)
        //    {
        //        string label = node.position.ToString() + "\nID: " + node.id + "\n Peso: " + node.weight;
        //        Handles.Label(pos + offsetLabel, label, style);
        //    }
        //}
        //
        //
        //return;
        foreach (Node node in map)
        {
            bool dontDraw = false;
            foreach (Vector3Int pos in path)
            {
                if (node.position == pos)
                {
                    dontDraw = true;
                    break;
                }
            }
        
            switch (node.state)
            {
                case Node.NodeState.Open:
                    newColor = Color.blue;
                    break;
                case Node.NodeState.Closed:
                    newColor = Color.red;
                    break;
                case Node.NodeState.Ready:
                    newColor = Color.green;
                    break;
                case Node.NodeState.Block:
                    newColor = Color.black;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        
            Vector3 nodePosition = node.position;
        
            newColor.a = alphaColor;
            Gizmos.color = newColor;
        
            if (!dontDraw)
                Gizmos.DrawCube(nodePosition, new Vector3(1, 0, 1));
        
            if (showLabel)
            {
                string label = node.position.ToString() + "\nID: " + node.id + "\n Peso: " + node.weight;
                Handles.Label(nodePosition + offsetLabel, label, style);
            }
        
        
            Gizmos.color = Color.black;
            Gizmos.DrawWireCube(nodePosition, new Vector3(1, 0, 1));
        }
        
        newColor = Color.cyan;
        newColor.a = alphaColor;

        Gizmos.color = newColor;
        foreach (Vector3Int pos in path)
        {
            Gizmos.DrawCube(pos, new Vector3(1, 0, 1));

        }
    }
#endif
}