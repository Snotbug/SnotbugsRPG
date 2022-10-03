using System.Collections.Generic;
using UnityEngine;

public class Status : MonoBehaviour
{
    [field : SerializeField] public StatusBase Base { get; private set; }
    [field : SerializeField] public StatusUI UI { get; private set; }

    public Creature Owner { get; private set; }
    
    public Dictionary<string, Stat> Stats { get; private set; }

    public Stat Duration { get { return Stats["Duration"]; }}

    public List<Effect> TriggeredEffects { get; private set; }
    
    public void SetBase(Creature owner)
    {
        Owner = owner;

        Stats = new Dictionary<string, Stat>();
        foreach(StatBase Stat in Base.Stats)
        {
            Stats.Add(Stat.Definition.Name, new Stat(Stat.Definition, Stat.Current, Stat.Max));
        };

        TriggeredEffects = new List<Effect>();
        foreach(EffectBase effectBase in Base.TriggeredEffects)
        {
            Effect effect = new Effect(effectBase, Owner);
            effectBase.Trigger.RegisterEffect(effect);
            TriggeredEffects.Add(effect);
        }

        UI.SetUI(this);
    }

    public Stat FindStat(StatDefinition stat)
    {
        return Stats.ContainsKey(stat.Name) ? Stats[stat.Name] : null;
    }

    public Stat FindStat(string name)
    {
        return Stats.ContainsKey(name) ? Stats[name] : null;
    }

    public void ModifyStat(string name, int amount)
    {
        Stats[name].Modify(amount);
        if(Duration.Current <= 0) { Owner.RemoveStatus(this); }
    }

    public void SetStat(string name, int value)
    {
        Stats[name].Set(value);
        if(Duration.Current <= 0) { Owner.RemoveStatus(this); }
    }

    public void ModifyMaxStat(string name, int amount)
    {
        Stats[name].ModifyMax(amount);
        if(Duration.Current <= 0) { Owner.RemoveStatus(this); }
    }

    public void SetMaxStat(string name, int value)
    {
        Stats[name].SetMax(value);
        if(Duration.Current <= 0) { Owner.RemoveStatus(this); }
    }

    public void OnDestroy()
    {
        foreach(Effect effect in TriggeredEffects) { effect.Base.Trigger.UnregisterEffect(effect); }
        if(UI != null) { Destroy(UI.gameObject); }
    }
}
