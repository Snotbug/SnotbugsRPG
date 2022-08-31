using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Status", menuName = "Status")]
public class StatusBase : ScriptableObject
{
    [field : SerializeField] public string Name { get; private set; }
    [field : SerializeField] public Sprite Icon { get; private set; }
    [field : SerializeField] public string Description { get; private set; }
    
    [field : SerializeField] public List<StatBase> Stats { get; private set; }
    [field : SerializeField] public List<EffectBase> TriggeredEffects { get; private set; }
}