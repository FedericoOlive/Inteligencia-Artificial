using System;
using System.Collections.Generic;

public class State
{
    public Action OnEntryBehaviour;
    public List<Action> behaviours;
    public Action OnExitBehaviour;
}