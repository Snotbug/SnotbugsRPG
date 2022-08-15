using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Trigger", menuName = "Trigger")]
public class EffectTrigger : ScriptableObject
{
    private readonly List<Effect> Effects = new List<Effect>();

    public void TriggerEffect(Creature source, Creature target)
    {
        for(int i = Effects.Count - 1; i >= 0; i--)
        {
            switch(Effects[i].Base.TriggerType)
            {
                case TriggerType.None:
                    Effects[i].QueueEffect(false);
                    break;
                case TriggerType.Source:
                    if(source != null && Effects[i].Owner == source) { Effects[i].QueueEffect(false); }
                    break;
                case TriggerType.Target:
                    if(target != null && Effects[i].Owner == target) { Effects[i].QueueEffect(false); }
                    break;
                default:
                    break;
            }
        }
    }

    public void RegisterEffect(Effect effect) { if(!Effects.Contains(effect)) { Effects.Add(effect); }}

    public void UnregisterEffect(Effect effect) { if(Effects.Contains(effect)) { Effects.Remove(effect); }}
}