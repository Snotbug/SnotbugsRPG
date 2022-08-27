using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    [field : SerializeField] public State Current { get; private set; }
    [field : SerializeField] public State Previous { get; private set; }

    public bool Pending = false;

    public void Change(State state)
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
        Change(Previous);
    }
}
