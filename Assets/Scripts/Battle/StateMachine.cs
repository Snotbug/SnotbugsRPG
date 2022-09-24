using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    public IState Current { get; private set; }
    public IState Previous { get; private set; }

    public bool Pending = false;

    public StateMachine()
    {
        
    }

    public void Switch(IState state)
    {
        if(Current == state || Pending) { return; }
        
        Pending = true;
        if(Current != null) { Current.Exit(); }
        if(Previous != null) { Previous = Current; }
        Current = state;
        if(Current != null) { Current.Enter(); }
        Pending = false;
    }

    public void Revert()
    {
        if(Previous == null) { return; }
        Switch(Previous);
    }
}
