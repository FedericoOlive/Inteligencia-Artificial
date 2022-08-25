using System;
using System.Collections.Generic;
using UnityEngine;

// Falta borrar una linea de código que la mostró en clase

public class Pathfinding
{
    public enum Methods
    {
        BreadthFirst,
        DephFirst
    }

    public Methods methods;

    private List<int> openNodesId = new List<int>();
    private List<int> closedNodesId = new List<int>();

    public List<Vector2Int> GetPath (Node[] map, Node origin, Node destination)
    {
        openNodesId.Add(origin.id);
        Node currentNode = origin;

        while (currentNode.position != destination.position)
        {
            currentNode = GetNextNode(map, currentNode);
            if (currentNode == null)
                return new List<Vector2Int>();
            for (int i = 0; i < currentNode.adjacentNodeIds.Count; i++)
            {
                if (currentNode.adjacentNodeIds[i] != -1)
                {
                    if (map[currentNode.adjacentNodeIds[i]].state == Node.NodeState.Ready)
                    {
                        map[currentNode.adjacentNodeIds[i]].Open(currentNode.id);
                        openNodesId.Add(map[currentNode.adjacentNodeIds[i]].id);
                    }
                }
            }

            currentNode.state = Node.NodeState.Closed;
            openNodesId.Remove(currentNode.id);
            closedNodesId.Add(currentNode.id);
        }

        // destination.Open(currentNode.id); // Porque no funciona? Si no abro la ultima, no se setea como abierta
        List<Vector2Int> path = GeneratePath(map, currentNode);

        return path;
    }

    private List<Vector2Int> GeneratePath (Node[] map, Node current)
    {
        List<Vector2Int> path = new List<Vector2Int>();

        while (current.openerId != -1)
        {
            path.Add(current.position);
            current = map[current.openerId];
        }

        path.Add(current.position);
        path.Reverse();

        return path;
    }

    private Node GetNextNode (Node[] map, Node currentNode)
    {
        switch (methods)
        {
            case Methods.BreadthFirst:
                return map[openNodesId[0]];
            case Methods.DephFirst:
                return map[openNodesId[^1]];
            default:
                return null;
        }
    }
}