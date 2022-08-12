using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AntBehaviour", menuName = "State/AntBehaviour", order = 1)]

public class AntBehaviour : ScriptableObject
{
    public States sourceState;
    public List<Relation> relations = new List<Relation>();
}

[System.Serializable] public class Relation
{
    public Flags flag;
    public States destinationState;
    public ModeTransition modeTransition;

    public float timeDurarion = 1;
}

public enum ModeTransition
{
    Objetive,
    Time,
    Last
}