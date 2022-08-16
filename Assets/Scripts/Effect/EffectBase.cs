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
    [field : SerializeField] public List<StaticEffectData> Data { get; private set; }
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
public class StaticEffectData
{
    [field : SerializeField] public ActivationType ActivationType { get; private set; }
    [field : SerializeField] public List<StatBase> Scalings { get; private set; }
    [field : SerializeField] public UnityEvent<Creature, Creature, DynamicEffectData> Function { get; private set; }

    [field : SerializeField] public Creature Creature { get; private set; }
    [field : SerializeField] public StatBase Stat { get; private set; }
    [field : SerializeField] public Status Status { get; private set; }
    [field : SerializeField] public Spell Spell { get; private set; }
    [field : SerializeField] public Item Item { get; private set; }
    [field : SerializeField] public Equipment Equipment { get; private set; }
}

public enum ActivationType
{
    None,
    OnTrue,
    OnFalse
}