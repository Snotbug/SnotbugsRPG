using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterChoice : MonoBehaviour
{
    [field : SerializeField] public string Name { get; private set; }
    [field : SerializeField] public string Description { get; private set; }

    [field : SerializeField] public List<EncounterPrerequisite> Prerequisite { get; private set; }
    [field : SerializeField] public List<EncounterConsequence> Consequence { get; private set; }
}

[System.Serializable]
public class EncounterPrerequisite
{
    [field : SerializeField] public StatBase Stats { get; private set; }
    [field : SerializeField] public Spell Spells { get; private set; }
    [field : SerializeField] public Status Statuses { get; private set; }
    [field : SerializeField] public Item Item { get; private set; }
}

[System.Serializable]
public class EncounterConsequence
{
    [field : SerializeField] public List<StatBase> Stats { get; private set; }
    [field : SerializeField] public List<Spell> Spells { get; private set; }
    [field : SerializeField] public List<Status> Statuses { get; private set; }
    [field : SerializeField] public List<Item> Item { get; private set; }
}