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
    [field : SerializeField] public ChoiceData Requirements { get; private set; }
    [field : SerializeField] public UnityEvent<ChoiceData> Consequence { get; private set; }
}

[System.Serializable]
public class ChoiceData
{
    [field : SerializeField] public List<StatBase> Stats { get; private set; }
    [field : SerializeField] public List<Status> Status { get; private set; }
    [field : SerializeField] public List<Item> Items { get; private set; }
    [field : SerializeField] public List<Spell> Spells { get; private set; }
    [field : SerializeField] public List<Equipment> Equipment { get; private set; }
    [field : SerializeField] public List<Equipment> Encounter { get; private set; }
}