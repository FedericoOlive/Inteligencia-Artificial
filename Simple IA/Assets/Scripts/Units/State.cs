using System;
using System.Collections.Generic;

public class State
{
    public Action entryBehaviour;
    public List<Action> behaviours;
    public Action exitBehaviour;

    public State ()
    {
        behaviours = new List<Action>();
    }
}