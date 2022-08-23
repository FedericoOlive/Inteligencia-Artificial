using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public enum NodeState
    {
        Block,
        Open, // Abiertos por otro nodo pero no visitados
        Closed, // ya visitados
        Ready // no abiertos por nadie
    }

    public int id;
    public Vector2Int position;
    public List<int> adjacentNodeIds;
    public NodeState state;
    public int openerId;
    public bool isPosAvailable = true;

    public Node (int newId, Vector2Int newPos)
    {
        id = newId;
        position = newPos;
        adjacentNodeIds = NodeUtils.GetAdjacentsNodeIDs(newPos);
        Reset();
        Vector3 pos = new Vector3(position.x, position.y, 0);
        if (Physics.SphereCast(pos, 0.5f, Vector3.zero, out var hit))
        {
            state = NodeState.Block;
            Debug.Log("Nodo: " + position + " || Bloqueado por: " + hit.transform.name);
        }
    }

    public void Open (int newOpenerId)
    {
        state = NodeState.Open;
        openerId = newOpenerId;
    }

    public void Reset ()
    {
        state = NodeState.Ready;
        openerId = -1;
    }
}