using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [field : SerializeField] public ItemBase Base { get; private set; }
    [field : SerializeField] public ItemUI UI { get; private set; }

    public Creature Owner { get; private set; }

    public Dictionary<string, Stat> Stats { get; private set; }

    public Stat Quantity { get { return Stats["Quantity"]; }}
    
    public Effect ActivatedEffect { get; private set; }

    public void SetBase(Creature owner)
    {
        Owner = owner;

        Stats = new Dictionary<string, Stat>();
        foreach(StatBase Stat in Base.Stats)
        {
            Stats.Add(Stat.Definition.Name, new Stat(Stat.Definition, Stat.Current, Stat.Max));
        };

        ActivatedEffect = new Effect(Base.ActivatedEffect, Owner, this);

        UI.SetUI(this);
    }

    public void Activate()
    {
        ModifyStat(Quantity.Definition.Name, -1);
        ActivatedEffect.QueueEffect(true);
    }

    public Stat FindStat(StatDefinition stat) { return Stats.ContainsKey(stat.Name) ? Stats[stat.Name] : null; }

    public void ModifyStat(string name, int amount)
    {
        Stats[name].Modify(amount);
    }

    public void SetStat(string name, int value)
    {
        Stats[name].Set(value);
    }

    public void ModifyMaxStat(string name, int amount)
    {
        Stats[name].ModifyMax(amount);
    }

    public void SetMaxStat(string name, int value)
    {
        Stats[name].SetMax(value);
    }

    public void OnDestroy()
    {
        if(UI == null) { return; }
        Destroy(UI.gameObject);
    }
}