using UnityEngine;

public abstract class State : ScriptableObject
{
    public abstract void Enter();

    public abstract void Exit();
}
