using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Node
{
    public enum NodeState
    {
        Open, // Abiertos por otro nodo pero no visitados
        Closed, // ya visitados
        Ready, // no abiertos por nadie
        Block
    }

    [HideInInspector] public int id;
    public Vector3Int position;
    [HideInInspector] public List<int> adjacentNodeIds;
    public NodeState state;
    [HideInInspector] public int openerId;
    [HideInInspector] public bool isPosAvailable = true;

    public int weight = 1;
    public int totalWeight;

    public Node (int newId, Vector3Int newPos)
    {
        id = newId;
        position = newPos;
        adjacentNodeIds = NodeUtils.GetAdjacentsNodeIDs(newPos);
        Reset();
        Vector3 pos = new Vector3(position.x, position.z, 0);
        if (Physics.SphereCast(pos, 0.5f, Vector3.zero, out var hit))
        {
            state = NodeState.Block;
            Debug.Log("Nodo: " + position + " || Bloqueado por: " + hit.transform.name);
        }
    }

    public void Open (int newOpenerId, int parentWeight)
    {
        state = NodeState.Open;
        openerId = newOpenerId;
        weight += parentWeight;
    }

    public void Reset ()
    {
        if (state == NodeState.Block)
            return;
        state = NodeState.Ready;
        openerId = -1;
        weight = 1;
        totalWeight = 1;
    }
}