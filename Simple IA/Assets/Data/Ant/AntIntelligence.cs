using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "AntIntelligence", menuName = "State/Intelligence", order = 1)]
public class AntIntelligence : ScriptableObject
{
    public List<AntBehaviour> antIntelligence = new List<AntBehaviour>();
    public Dictionary<int, List<Action>> behaviours;
}

public enum States
{
    Idle,
    Mining,
    GoToMine,
    GoToAnthill,
    Last
}

public enum Flags
{
    OnFullInventory,
    OnReachMine,
    OnReachDeposit,
    OnEmpyMine,
    Last
}