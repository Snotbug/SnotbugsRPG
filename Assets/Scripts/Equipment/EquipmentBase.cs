using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Equipment", menuName = "Equipment")]
public class EquipmentBase : ScriptableObject
{
    [field : SerializeField] public string Name { get; private set; }
    [field : SerializeField] public string Description { get; private set; }
    [field : SerializeField] public EquipmentType Type { get; private set; }

    [field : SerializeField] public List<StatBase> Requirement { get; private set; }
    [field : SerializeField] public List<StatBase> Modifiers { get; private set; }
    [field : SerializeField] public List<StatBase> Stats { get; private set; }

    [field : SerializeField] public List<EffectBase> TriggeredEffects { get; private set; }
}

public enum EquipmentType
{
    Weapon,
    Armor,
    Relic
}