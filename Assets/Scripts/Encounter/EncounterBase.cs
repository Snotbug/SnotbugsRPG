using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Encounter", menuName = "Encounter")]
public class EncounterBase : ScriptableObject
{
    [field : SerializeField] public string Name { get; private set; }
    [field : SerializeField] public string Description { get; private set; }

    [field : SerializeField] public List<ChoiceBase> Choices { get; private set; }
}

[System.Serializable]
public class ChoiceBase
{
    [field : SerializeField] public string Name { get; private set; }
    [field : SerializeField] public string Description { get; private set; }

    [field : SerializeField] public EncounterBase NextEncounter { get; private set; }
    [field : SerializeField] public BattleUI Battle { get; private set; }

    [field : SerializeField] public ChoiceData Requirements { get; private set; }
    [field : SerializeField] public ChoiceConsequence Consequence { get; private set; }
}

[System.Serializable]
public class ChoiceConsequence
{
    [field : SerializeField] public UnityEvent<ChoiceData> Function { get; private set; }
    [field : SerializeField] public ChoiceData Data { get; private set; }
}

[System.Serializable]
public class ChoiceData
{
    [field : SerializeField] public List<StatBase> Stats { get; private set; }
    [field : SerializeField] public Status Status { get; private set; }
    [field : SerializeField] public Item Item { get; private set; }
    [field : SerializeField] public Spell Spell { get; private set; }
    [field : SerializeField] public Equipment Equipment { get; private set; }
}