using UnityEngine;

public class Stat
{
    public StatDefinition Definition { get; private set; }

    public int Current { get; private set; }
    public int Max { get; private set; }

    public Stat(StatDefinition definition, int current, int max)
    {
        Definition = definition;
        this.SetMax(max);
        this.Set(current);
    }

    public void Modify(int amount)
    {
        if(Definition.IsResource) { Current = Mathf.Clamp(Current + amount, 0, Max); }
        else { Current = Current + amount < 0 ? 0 : Current + amount; }
    }

    public void Set(int value)
    {
        int amount = value - Current;
        Modify(amount);
    }

    public void ModifyMax(int amount)
    {
        Max = Max + amount < 0 ? 0 : Max + amount;
        if(Definition.IsResource) { if (Current > Max) { Current = Max; }}
        else { Modify(amount); }
    }

    public void SetMax(int value)
    {
        int amount = value - Max;
        ModifyMax(amount);
    }
}
