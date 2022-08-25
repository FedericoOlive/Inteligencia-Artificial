using System.Collections.Generic;
using UnityEngine;

// Falta borrar una linea de código que la mostró en clase

public class Pathfinding
{
    public enum Methods
    {
        BreadthFirst,
        DephFirst,
        Dijkstra,
        AStar
    }

    private Methods methods = Methods.AStar;

    private List<int> openNodesId = new List<int>();
    private List<int> closedNodesId = new List<int>();
    private Vector2Int destinationPosition;

    public List<Vector2Int> GetPath (Node[] map, Node origin, Node destination)
    {
        openNodesId.Add(origin.id);
        Node currentNode = origin;
        destinationPosition = destination.position;

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
                        map[currentNode.adjacentNodeIds[i]].Open(currentNode.id, currentNode.totalWeight);
                        openNodesId.Add(map[currentNode.adjacentNodeIds[i]].id);
                    }
                }
            }

            currentNode.state = Node.NodeState.Closed;
            openNodesId.Remove(currentNode.id);
            closedNodesId.Add(currentNode.id);
        }

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
            case Methods.Dijkstra:
            {
                Node node = null;
                int currentMaxWeight = int.MaxValue;
                for (int i = 0; i < openNodesId.Count; i++)
                {
                    if (map[openNodesId[i]].totalWeight < currentMaxWeight)
                    {
                        node = map[openNodesId[i]];
                        currentMaxWeight = map[openNodesId[i]].totalWeight;
                    }
                }

                return node;
            }
            case Methods.AStar:
            {
                Node node = null;
                int currentMaxWeightAndDistance = int.MaxValue;
                for (int i = 0; i < openNodesId.Count; i++)
                {
                    if (map[openNodesId[i]].totalWeight + GetManhattanDistance(map[openNodesId[i]].position, destinationPosition) < currentMaxWeightAndDistance)
                    {
                        node = map[openNodesId[i]];
                        currentMaxWeightAndDistance = map[openNodesId[i]].totalWeight + GetManhattanDistance(map[openNodesId[i]].position, destinationPosition);
                    }
                }

                return node;
            }

            default:
                return null;
        }
    }

    private int GetManhattanDistance (Vector2Int origin, Vector2Int destination)
    {
        int distanceX = Mathf.Abs(origin.x - destination.x);
        int distanceY = Mathf.Abs(origin.y - destination.y);

        return distanceX + distanceY;
    }
}