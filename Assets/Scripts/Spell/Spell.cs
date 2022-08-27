using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Spell : MonoBehaviour
{
    [field : SerializeField] public SpellBase Base { get; private set; }
    [field : SerializeField] public SpellUI UI { get; private set; }

    public Creature Owner { get; private set; }
    
    public Dictionary<string, Stat> Stats { get; private set; }

    public Stat Cooldown { get { return Stats["Cooldown"]; }}
    
    public Effect ActivatedEffect { get; private set; }
    public List<Effect> TriggeredEffects { get; private set; }

    public void SetBase(Creature owner)
    {
        Owner = owner;

        Stats = new Dictionary<string, Stat>();
        foreach(StatBase stat in Base.Stats)
        {
            Stats.Add(stat.Definition.Name, new Stat(stat.Definition, stat.Current, stat.Max));
        };

        ActivatedEffect = new Effect(Base.ActivatedEffect, Owner);

        TriggeredEffects = new List<Effect>();
        foreach(EffectBase effectBase in Base.TriggeredEffects)
        {
            Effect effect = new Effect(effectBase, Owner);
            effectBase.Trigger.RegisterEffect(effect);
            TriggeredEffects.Add(effect);
        }

        UI.SetUI(this);
    }

    public void ActivateQueued()
    {
        Debug.Log($"{this.Base.Name}'s owner is null {this.Owner == null}");
        Owner.PayCost(Base.Costs);
        SetStat(Cooldown.Definition.Name, Cooldown.Max);
        ActivatedEffect.QueueEffect(true);
    }

    public Stat FindStat(StatDefinition stat)
    {
        return Stats.ContainsKey(stat.Name) ? Stats[stat.Name] : null;
    }

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
        foreach(Effect effect in TriggeredEffects) { effect.Base.Trigger.UnregisterEffect(effect); }
        if(UI != null) { Destroy(UI.gameObject); }
    }
}