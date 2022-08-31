using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class NodeGenerator : MonoBehaviour
{
    public Vector3Int mapSize;
    [SerializeField] private Node[] map;
    private Pathfinding pathfinding;

    public Vector3Int startPos = new Vector3Int();
    public Vector3Int endPos = new Vector3Int();
    public bool findPath;

    [SerializeField] private bool showLabel;
    [SerializeField] private int sizeLabel = 10;
    [SerializeField] private Vector3 offsetLabel = new Vector3(-0.45f, -0.45f, 0);

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

        map[NodeUtils.PositionToIndex(new Vector3Int(1, 0, 0))].state = Node.NodeState.Block;
        map[NodeUtils.PositionToIndex(new Vector3Int(3, 0, 1))].state = Node.NodeState.Block;
        map[NodeUtils.PositionToIndex(new Vector3Int(1, 0, 1))].SetWeight(20);
        map[NodeUtils.PositionToIndex(new Vector3Int(1, 0, 2))].SetWeight(20);
        map[NodeUtils.PositionToIndex(new Vector3Int(1, 0, 3))].SetWeight(20);
        map[NodeUtils.PositionToIndex(new Vector3Int(1, 0, 4))].SetWeight(20);
        map[NodeUtils.PositionToIndex(new Vector3Int(1, 0, 5))].SetWeight(20);
        map[NodeUtils.PositionToIndex(new Vector3Int(1, 0, 6))].SetWeight(20);
        map[NodeUtils.PositionToIndex(new Vector3Int(1, 0, 7))].SetWeight(20);
        map[NodeUtils.PositionToIndex(new Vector3Int(1, 0, 8))].SetWeight(20);
        map[NodeUtils.PositionToIndex(new Vector3Int(1, 0, 9))].SetWeight(20);
        map[NodeUtils.PositionToIndex(new Vector3Int(1, 0, 10))].SetWeight(20);
        map[NodeUtils.PositionToIndex(new Vector3Int(1, 0, 11))].SetWeight(20);

        List<Vector3Int> path = pathfinding.GetPath(map,
            map[NodeUtils.PositionToIndex(startPos)],
            map[NodeUtils.PositionToIndex(endPos)]);

        for (int i = 0; i < path.Count; i++)
        {
            Debug.Log(path[i]);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos ()
    {
        if (map == null)
            return;
        GUIStyle style = new GUIStyle() {fontSize = sizeLabel};
        Color newColor = Color.black;

        foreach (Node node in map)
        {
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
                    newColor = Color.magenta;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Vector3 worldPosition = new Vector3(node.position.x, 0, node.position.z);

            newColor.a = 0.4f;
            Gizmos.color = newColor;

            Gizmos.DrawCube(worldPosition, new Vector3(1, 0, 1));

            if (showLabel)
            {
                string label = node.position.ToString() + "\nID: " + node.id + "\n Peso: " + node.weight;
                Handles.Label(worldPosition + offsetLabel, label, style);
            }


            Gizmos.color = Color.black;
            Gizmos.DrawWireCube(worldPosition, new Vector3(1, 0, 1));
        }
    }
#endif
}