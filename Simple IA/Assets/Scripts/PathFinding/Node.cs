using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Node
{
    public enum NodeState
    {
        Ready,  // No abiertos
        Open,   // Abierto
        Closed, // Visitados
        Block   // Bloqueados
    }

    [HideInInspector] public int id;
    public NodeState state;
    [HideInInspector] public int openerId;
    public Vector3Int position;
    [HideInInspector] public List<int> adjacentNodeIds;

    public int weight = 1;
    public int totalWeight;
    private int originalWeight = 1;

    public Node (int newId, Vector3Int newPos)
    {
        Reset();
        id = newId;
        position = newPos;
        adjacentNodeIds = NodeUtils.GetAdjacentsNodeIDs(newPos);
        originalWeight = weight;

        Vector3 pos = new Vector3(position.x, position.z, 0);
        if (Physics.SphereCast(pos, 0.5f, Vector3.zero, out var hit))
        {
            state = NodeState.Block;
            Debug.Log("Nodo: " + position + " || Bloqueado por: " + hit.transform.name);
        }
    }
    public void SetWeight(int weight)
    {
        this.weight = weight;
        originalWeight = weight;
    }
    public void Open (int newOpenerId, int parentWeight)
    {
        state = NodeState.Open;
        openerId = newOpenerId;
        totalWeight = parentWeight + weight;
    }

    public void Reset ()
    {
        if (this.state == NodeState.Block) 
            return;

        state = NodeState.Ready;
        openerId = -1;
        weight = originalWeight;
    }
}