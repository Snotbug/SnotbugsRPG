using System;
using System.Collections.Generic;

public class Effect
{
    public EffectBase Base { get; private set; }

    public Creature Owner { get; private set; }
    public Creature Target { get; set; }

    public Effect(EffectBase effectBase, Creature owner)
    {
        Base = effectBase;
        Owner = owner;
    }

    public void QueueEffect(bool sendTrigger)
    {
        foreach(Subeffect subeffect in Base.Subeffects)
        {
            List<Creature> Targets = new List<Creature>();
            if (subeffect.TargetType == TargetType.None) { if(Target != null) { Targets.Add(Target); }}
            else { Targets = BattleManager.current.TargetController.FindTargets(Owner, subeffect.TargetType); }

            foreach(Creature target in Targets)
            {
                foreach(BaseEffectData data in subeffect.Data)
                {
                    EffectData Data = new EffectData(data, Owner, target, sendTrigger);
                    BattleManager.current.EffectController.Enqueue(Data);
                }
            }
        }
    }
}

public class EffectData
{
    public BaseEffectData Base { get; private set; }
    public bool SendTrigger { get; private set; }
    public Action OnComplete { get; set; }

    public Creature Source { get; private set; }
    public Creature Target { get; private set; }

    public StatBase Stat { get; set; }    
    public Creature Creature { get; set; }
    public Status Status { get; set; }
    public Spell Spell { get; set; }

    public EffectData(BaseEffectData baseData, Creature source, Creature target, bool sendTrigger)
    {
        Base = baseData;
        Source = source;
        Target = target;
        SendTrigger = sendTrigger;

        Stat = Base.Stat;
        Creature = Base.Creature;
        Status = Base.Status;
        Spell = Base.Spell;
    }
}