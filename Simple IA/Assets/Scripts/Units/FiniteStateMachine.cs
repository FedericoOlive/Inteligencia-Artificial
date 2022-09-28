using System;
using System.Collections.Generic;
using UnityEngine;

public class FiniteStateMachine
{
    private States[,] relations;
    private Dictionary<States, State> behaviours;
    public Action entryBehaviour;
    public Action exitBehaviour;

    public FiniteStateMachine (States states, Flags flags, Action onEntryBehaviour = null, Action onExitBehaviour = null)
    {
        relations = new States[(int) States.Last, (int) Flags.Last];
        for (int i = 0; i < (int) States.Last; i++)
        {
            for (int j = 0; j < (int) Flags.Last; j++)
            {
                relations[i, j] = States.Last;
            }
        }

        entryBehaviour = onEntryBehaviour;
        exitBehaviour = onExitBehaviour;

        behaviours = new Dictionary<States, State>();
    }
    
    public void SetRelation (States sourceState, Flags flag, States destinationState)
    {
        relations[(int) sourceState, (int) flag] = destinationState;
    }

    public void SetFlag (ref States currentState, Flags flag)
    {
        if (behaviours.ContainsKey(currentState))
            ExitBehaviourState(currentState);

        if (relations[(int) currentState, (int) flag] != States.Last)
        {
            currentState = relations[(int) currentState, (int) flag];
            EntryBehaviourState(currentState);
        }
        else
        {
            if (flag == Flags.Last)
                Debug.Log("Exit FSM.");
            else
                Debug.Log("No existe connexión entre: " + currentState + " y " + flag);
        }
    }

    private void ExitBehaviourState (States currentState)
    {
        if (behaviours[currentState].exitBehaviour != null)
            behaviours[currentState].exitBehaviour.Invoke();
    }

    private void EntryBehaviourState (States currentState)
    {
        if (behaviours[currentState].entryBehaviour != null)
            behaviours[currentState].entryBehaviour.Invoke();
    }

    public void AddBehaviour (States state, Action behaviour, Action onEntryBehaviour = null, Action onExitBehaviour = null)
    {
        if (behaviours.ContainsKey(state))
            behaviours[state].behaviours.Add(behaviour);
        else
        {
            State newState = new State();
            newState.behaviours.Add(behaviour);
            newState.entryBehaviour = onEntryBehaviour;
            newState.exitBehaviour = onExitBehaviour;
            
            behaviours.Add(state, newState);
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

    ForceIndicator,
    ForceGoingToPosition,
    ForceGoingToAnthill,
    ForceGoingToIdle,
    ForceToHarvasting,

    Last
}

public enum Flags
{
    OnFullInventory,
    OnArriveResource,
    OnArriveWithResource,
    OnReceiveResource,
    OnEmptyInventory,

    ForceIndicator,
    ForceToPosition,
    ForceToIdle,
    ForceToAnthill,
    ForceToResource,

    Last
}