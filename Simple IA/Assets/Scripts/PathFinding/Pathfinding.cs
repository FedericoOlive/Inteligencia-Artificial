using System.Collections.Generic;
using UnityEngine;

public class Pathfinding
{
    public List<Vector2Int> GetPath (Node[] map, Node origin, Node destination)
    {
        Node currentNode = origin;
        while (currentNode.position != destination.position)
        {
            currentNode.state = Node.NodeState.Closed;

            for (int i = 0; i < currentNode.adjacentNodeIds.Count; i++)
            {
                if (currentNode.adjacentNodeIds[i] != -1)
                {
                    map[currentNode.adjacentNodeIds[i]].Open(currentNode.id);
                }
            }

            currentNode = GetNextNode(map, currentNode);
            if (currentNode == null)
                return new List<Vector2Int>();
        }

        List<Vector2Int> path = GeneratePath(map, currentNode);
        foreach (Node node in map)
        {
            node.Reset();
        }

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

        path.Reverse();

        return path;
    }

    private Node GetNextNode (Node[] map, Node currentNode)
    {
        for (int i = 0; i < currentNode.adjacentNodeIds.Count; i++)
        {
            if (currentNode.adjacentNodeIds[i] != -1)
            {
                if (map[currentNode.adjacentNodeIds[i]].state == Node.NodeState.Open)
                {
                    if (map[currentNode.adjacentNodeIds[i]].openerId == currentNode.id)
                    {
                        return map[currentNode.adjacentNodeIds[i]];
                    }
                }
            }
        }

        if (currentNode.openerId == -1)
            return null;

        return GetNextNode(map, map[currentNode.openerId]);
    }
}