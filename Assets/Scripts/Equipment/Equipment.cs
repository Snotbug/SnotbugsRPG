using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : MonoBehaviour
{
    [field : SerializeField] public EquipmentBase Base { get; private set; }
    [field : SerializeField] public EquipmentUI UI { get; private set; }

    public Creature Owner { get; private set; }
    
 
    
    public List<Effect> TriggeredEffects { get; private set; }

    public void SetBase(Creature owner)
    {
        Owner = owner;

    

        TriggeredEffects = new List<Effect>();
        foreach(EffectBase effectBase in Base.TriggeredEffects)
        {
            Effect effect = new Effect(effectBase, Owner);
            effectBase.Trigger.RegisterEffect(effect);
            TriggeredEffects.Add(effect);
        }

        UI.SetUI(this);
    }

   

    public void OnDestroy()
    {
        foreach(Effect effect in TriggeredEffects) { effect.Base.Trigger.UnregisterEffect(effect); }
        if(UI != null) { Destroy(UI.gameObject); }
    }
}
