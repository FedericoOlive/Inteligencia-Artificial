using System.Collections.Generic;
using UnityEngine;


public class Pathfinding
{
    public List<Vector3Int> GetPath (Node[] map, Node origin, Node destination)
    {
        List<int> openNodesId = new List<int>();
        List<int> closedNodesId = new List<int>();
        Vector3Int destinationPosition;

        openNodesId.Add(origin.id);
        destinationPosition = destination.position;
        Node currentNode = origin;

        while (currentNode.position != destination.position)
        {
            currentNode = GetNextNode(map, currentNode, ref destinationPosition, openNodesId);
            if (currentNode == null)
                return new List<Vector3Int>();

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
        }

        List<Vector3Int> path = GeneratePath(map, currentNode);

        //foreach (Node node in map)
        //{
        //    node.Reset();
        //}
        //openNodesId.Clear();
        return path;
    }

    private List<Vector3Int> GeneratePath (Node[] map, Node current)
    {
        List<Vector3Int> path = new List<Vector3Int>();

        while (current.openerId != -1)
        {
            path.Add(current.position);
            current = map[current.openerId];
        }

        path.Add(current.position);
        path.Reverse();

        return path;
    }

    private Node GetNextNode (Node[] map, Node currentNode, ref Vector3Int destinationPosition, List<int> openNodesId)
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

    private int GetManhattanDistance (Vector3Int origin, Vector3Int destination)
    {
        int distanceX = Mathf.Abs(origin.x - destination.x);
        int distanceY = Mathf.Abs(origin.z - destination.z);

        return distanceX + distanceY;
    }
}