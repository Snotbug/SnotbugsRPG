using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class ChoiceBase
{
    [field : SerializeField] public string Name { get; private set; }
    [field : SerializeField] public string PreDescription { get; private set; }
    [field : SerializeField] public string Description { get; private set; }

    [field : SerializeField] public EncounterBase NextEncounter { get; private set; }
    [field : SerializeField] public BattleLayout BattleLayout { get; private set; }

    [field : SerializeField] public BaseChoiceData Requirements { get; private set; }
    [field : SerializeField] public List<ChoiceConsequence> Consequences { get; private set; }
}

[System.Serializable]
public class ChoiceConsequence
{
    [field : SerializeField] public UnityEvent<ChoiceData> Function { get; private set; }
    [field : SerializeField] public BaseChoiceData Data { get; private set; }
}

[System.Serializable]
public class BaseChoiceData
{
    [field : SerializeField] public List<StatBase> Stats { get; private set; }
    [field : SerializeField] public Status Status { get; private set; }
    [field : SerializeField] public Spell Spell { get; private set; }
}