using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Spell : MonoBehaviour
{
    [field : SerializeField] public SpellBase data { get; private set; }
    [field : SerializeField] public SpellUI UI { get; private set; }

    public Creature Owner { get; private set; }
    
 
    
    
    public Effect ActivatedEffect { get; private set; }
    public List<Effect> TriggeredEffects { get; private set; }

    public void SetBase(Creature owner)
    {
        Owner = owner;

        ActivatedEffect = new Effect(data.ActivatedEffect, Owner);

        TriggeredEffects = new List<Effect>();
        foreach(EffectBase effectBase in data.TriggeredEffects)
        {
            Effect effect = new Effect(effectBase, Owner);
            effectBase.Trigger.RegisterEffect(effect);
            TriggeredEffects.Add(effect);
        }

        UI.SetUI(this);
    }

    public void ActivateQueued()
    {
       // Owner.PayCost(data.Costs);
    
        ActivatedEffect.QueueEffect(true);
    }

   

    public void OnDestroy()
    {
        foreach(Effect effect in TriggeredEffects) { effect.Base.Trigger.UnregisterEffect(effect); }
        if(UI != null) { Destroy(UI.gameObject); }
    }
}