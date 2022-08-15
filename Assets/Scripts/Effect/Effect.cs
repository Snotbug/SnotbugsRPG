using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System;

public class Effect
{
    public EffectBase Base { get; private set; }

    public Creature Owner { get; private set; }
    public Status ParentStatus { get; private set; }
    public Spell ParentSpell { get; private set; }
    public Item ParentItem { get; private set; }
    public Creature Target { get; set; }

    public Effect(EffectBase effectBase, Creature owner, Spell spell)
    {
        Base = effectBase;
        Owner = owner;
        ParentSpell = spell;
    }

    public Effect(EffectBase effectBase, Creature owner, Status status)
    {
        Base = effectBase;
        Owner = owner;
        ParentStatus = status;
    }

    public Effect(EffectBase effectBase, Creature owner, Item item)
    {
        Base = effectBase;
        Owner = owner;
        ParentItem = item;
    }

    public void QueueEffect(bool sendTrigger)
    {
        foreach(Subeffect subeffect in Base.Subeffects)
        {
            List<Creature> Targets = new List<Creature>();
            if (subeffect.TargetType == TargetType.None && Target != null) { Targets.Add(Target); }
            else { Targets = BattleManager.current.TargetController.FindTargets(Owner, subeffect.TargetType); }

            foreach(Creature target in Targets)
            {
                DynamicEffectData Data = new DynamicEffectData(this, Owner, target, sendTrigger);
                foreach(StaticEffectData data in subeffect.Data)
                {
                    Data.SetBase(data);
                    switch(data.ActivationType)
                    {
                        case ActivationType.None:
                            BattleManager.current.EffectController.Enqueue(Data);
                            break;
                        case ActivationType.OnTrue:
                            if(Data.Found) { BattleManager.current.EffectController.Enqueue(Data); }
                            break;
                        case ActivationType.OnFalse:
                            if(!Data.Found) { BattleManager.current.EffectController.Enqueue(Data); }
                            break;
                    }
                }
            }
        }
    }
}

public class DynamicEffectData
{
    public StaticEffectData Base { get; private set; }
    public bool SendTrigger { get; private set; }
    public Action OnComplete { get; set; }

    public Creature Owner { get; private set; }
    public Creature Target { get; private set; }

    public Status ParentStatus { get; private set; }
    public Spell ParentSpell { get; private set; }
    public Item ParentItem { get; private set; }
    
    public bool Found { get; set; }
    public int Value { get; set; }

    public StatBase Stat { get; set; }    
    public Creature Creature { get; set; }
    public Status Status { get; set; }
    public Spell Spell { get; set; }
    public Item Item { get; set; }

    public DynamicEffectData(Effect effect, Creature owner, Creature target, bool sendTrigger)
    {
        Owner = owner;
        Target = target;

        SendTrigger = sendTrigger;

        ParentStatus = effect.ParentStatus;
        ParentSpell = effect.ParentSpell;
        ParentItem = effect.ParentItem;

        Found = false;
        Value = 0;
    }

    public void SetBase(StaticEffectData baseData)
    {
        Base = baseData;

        if(Base.Stat.Definition != null) { Stat = Base.Stat; }
        if(Base.Creature != null) { Creature = Base.Creature; }
        if(Base.Status != null) { Status = Base.Status; }
        if(Base.Spell != null) { Spell = Base.Spell; }
        if(Base.Item != null) { Item = Base.Item; }
    }
}