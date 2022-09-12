using System;
using System.Collections.Generic;
using UnityEngine;

public class FiniteStateMachine
{
    private States[,] relations;
    private Dictionary<States, List<Action>> behaviours;

    public FiniteStateMachine (States states, Flags flags)
    {
        relations = new States[(int) States.Last, (int) Flags.Last];
        for (int i = 0; i < (int) States.Last; i++)
        {
            for (int j = 0; j < (int) Flags.Last; j++)
            {
                relations[i, j] = States.Last;
            }
        }

        behaviours = new Dictionary<States, List<Action>>();
    }

    public void SetRelation (States sourceState, Flags flag, States destinationState)
    {
        relations[(int) sourceState, (int) flag] = destinationState;
    }

    public void SetFlag (ref States currentState, Flags flag)
    {
        if (relations[(int) currentState, (int) flag] != States.Last)
        {
            currentState = relations[(int) currentState, (int) flag];
            return;
        }

        Debug.Log("No existe connexión entre: " + currentState + " y " + flag);
    }

    public void SetBehaviour (States state, Action behaviour)
    {
        List<Action> newBehaviours = new List<Action>();
        newBehaviours.Add(behaviour);

        if (behaviours.ContainsKey(state))
            behaviours[state] = newBehaviours;
        else
            behaviours.Add(state, newBehaviours);
    }

    public void AddBehaviour (States state, Action behaviour)
    {

        if (behaviours.ContainsKey(state))
            behaviours[state].Add(behaviour);
        else
        {
            List<Action> newBehaviours = new List<Action>();
            newBehaviours.Add(behaviour);
            behaviours.Add(state, newBehaviours);
        }
    }

    public void Update (ref States currentState)
    {
        if (behaviours.ContainsKey(currentState))
        {
            List<Action> actions = behaviours[currentState];
            if (actions != null)
            {
                for (int i = 0; i < actions.Count; i++)
                {
                    if (actions[i] != null)
                    {
                        actions[i].Invoke();
                    }
                }
            }
        }
    }
}

public enum States
{
    WaitingInstructions,
    Harvesting,
    GoingToResource,
    GoingToAnthill,
    Depositing,
    Last
}

public enum Flags
{
    OnFullInventory,
    OnArriveResource,
    OnArriveWithResource,
    OnReceiveResource,
    OnEmptyInventory,

    ForceToIdle,
    ForceToAnthill,
    ForceToHarvesting,

    Last
}