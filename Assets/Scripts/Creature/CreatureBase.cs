using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Creature", menuName = "Creature")]
public class CreatureBase : ScriptableObject
{
    [field : SerializeField] public string Name { get; private set; }

    [field : SerializeField] public List<StatBase> Stats { get; private set; }    
    [field : SerializeField] public List<Status> Statuses { get; private set; }
    [field : SerializeField] public List<Spell> Spells { get; private set; }
    [field : SerializeField] public List<Item> Items { get; private set; }
    [field : SerializeField] public List<Equipment> Equipments { get; private set; }
}
