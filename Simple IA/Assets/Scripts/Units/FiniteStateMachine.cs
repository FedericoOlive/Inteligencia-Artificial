using System;
using System.Collections.Generic;
using UnityEngine;

public class FiniteStateMachine
{
    private States[,] relations;
    private Dictionary<States, State> behaviours;

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

        behaviours = new Dictionary<States, State>();
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

    public void SetBehaviour (States state, Action behaviour, Action onEntryBehaviour = null, Action onExitBehaviour = null)
    {
        State newState = new State();
        newState.behaviours = new List<Action>();
        newState.behaviours.Add(behaviour);
        newState.OnEntryBehaviour = onEntryBehaviour;
        newState.OnExitBehaviour = onExitBehaviour;

        if (behaviours.ContainsKey(state))
            behaviours[state] = newState;
        else
            behaviours.Add(state, newState);
    }

    public void AddBehaviour (States state, Action behaviour, Action onEntryBehaviour = null, Action onExitBehaviour = null)
    {
        if (behaviours.ContainsKey(state))
        {
            behaviours[state].behaviours.Add(behaviour);
        }
        else
        {
            State newState = new State();
            newState.behaviours = new List<Action>();
            newState.behaviours.Add(behaviour);
            newState.OnEntryBehaviour = onEntryBehaviour;
            newState.OnExitBehaviour = onExitBehaviour;
            behaviours.Add(state, newState);
        }
    }

    public void Entry (ref States currentState)
    {
        if (behaviours.ContainsKey(currentState))
        {
            Action onExit = behaviours[currentState].OnEntryBehaviour;
            if (onExit != null)
            {
                onExit.Invoke();
            }
        }
    }

    public void Exit (ref States currentState)
    {
        if (behaviours.ContainsKey(currentState))
        {
            Action onExit = behaviours[currentState].OnExitBehaviour;
            if (onExit != null)
            {
                onExit.Invoke();
            }
        }
    }

    public void Update (ref States currentState)
    {
        if (behaviours.ContainsKey(currentState))
        {
            List<Action> actions = behaviours[currentState].behaviours;
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
    Idle,
    WaitingInstructions,
    Harvesting,
    GoingToResource,
    GoingToAnthill,
    Depositing,
    ForceGoingToPosition,
    ForceGoingToAnthill,
    ForceGoingToIdle,

    Last
}

public enum Flags
{
    OnFullInventory,
    OnArriveResource,
    OnArriveWithResource,
    OnReceiveResource,
    OnEmptyInventory,

    ForceToPosition,
    ForceToIdle,
    ForceToAnthill,
    ForceToHarvesting,

    Last
}