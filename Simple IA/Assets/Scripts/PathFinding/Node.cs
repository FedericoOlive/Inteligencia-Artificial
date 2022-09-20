using System;
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

    public int viewerId;

    [HideInInspector] public int id;
    public NodeState state;
    [HideInInspector] public int openerId;
    public Vector3Int position;
    [HideInInspector] public List<int> adjacentNodeIds;

    public int weight;
    public int totalWeight;

    public Node (int newId, Vector3Int newPos)
    {
        Reset();
        id = newId;
        position = newPos;
        adjacentNodeIds = NodeUtils.GetAdjacentsNodeIDs(newPos);
        SetNodeForTerrain();
    }

    void SetNodeForTerrain ()
    {
        TerrainCellType terrainCellType = TerrainTextureDetector.GetTerrainCellType(position);
        switch (terrainCellType)
        {
            case TerrainCellType.Grass:
                state = NodeState.Ready;
                break;
            case TerrainCellType.Sand:
                state = NodeState.Ready;
                break;
            case TerrainCellType.Rock:
            default:
                state = NodeState.Block;
                break;
        }

        weight = TerrainSettings.GetWeightByTerrainType(terrainCellType);
    }

    public void SetWeight (int weight)
    {
        this.weight = weight;
    }

    public void Open (int newOpenerId, int parentWeight)
    {
        //Debug.Log("Opened: " + id + " By: " + viewerId);
        state = NodeState.Open;
        openerId = newOpenerId;
        totalWeight = parentWeight + weight;
    }

    public void Reset ()
    {
        if (this.state == NodeState.Block)
            return;

        state = NodeState.Ready;
        viewerId = -1;
        openerId = -1;
    }
}