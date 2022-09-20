using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class EffectBase
{
    [field : SerializeField] public EffectTrigger Trigger { get; private set; }
    [field : SerializeField] public TriggerType TriggerType { get; private set; }
    [field : SerializeField] public List<Subeffect> Subeffects { get; private set; }
}

public enum TriggerType
{
    None,
    Source,
    Target
}

[System.Serializable]
public class Subeffect
{
    [field : SerializeField] public TargetType TargetType { get; private set; }
    [field : SerializeField] public List<BaseEffectData> Data { get; private set; }
}

public enum TargetType
{
    None,
    Self,
    Friend,
    Enemy,
    All
}


[System.Serializable]
public class BaseEffectData
{
    [field : SerializeField] public List<StatBase> Scalings { get; private set; }
    [field : SerializeField] public UnityEvent<EffectData> Function { get; private set; }

    [field : SerializeField] public Creature Creature { get; private set; }
    [field : SerializeField] public StatBase Stat { get; private set; }
    [field : SerializeField] public Status Status { get; private set; }
    [field : SerializeField] public Spell Spell { get; private set; }
}