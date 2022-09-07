using System.Collections.Generic;
using UnityEngine;

public class Status : MonoBehaviour
{
    [field : SerializeField] public StatusBase Base { get; private set; }
    [field : SerializeField] public StatusUI UI { get; private set; }

    public Creature owner { get; private set; }


    public void SetBase(Creature incomingOwner)
    {
        owner = incomingOwner;
        foreach(StatBase Stat in Base.Stats)
        {
          //  Stats.Add(Stat.Definition.Name, new Stat(Stat.Definition, Stat.Current, Stat.Max));
        };

      //  TriggeredEffects = new List<Effect>();
        foreach(EffectBase effectBase in Base.TriggeredEffects)
        {
            Effect effect = new Effect(effectBase, owner);
            effectBase.Trigger.RegisterEffect(effect);
        //    TriggeredEffects.Add(effect);
        }

        UI.SetUI(this);
    }

    public void OnDestroy()
    {
      //  foreach(Effect effect in TriggeredEffects) { effect.Base.Trigger.UnregisterEffect(effect); }
        if(UI != null) { Destroy(UI.gameObject); }
    }
}
