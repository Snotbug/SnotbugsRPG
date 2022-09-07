using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CreatureData_[Name]", menuName = "Game Data/Creature")]
public class CreatureData : ScriptableObject
{
    [field : SerializeField] public string name { get; private set; }
    public StatBlock statBlock; 
    [field : SerializeField] public List<Spell> Spells { get; private set; }
    [field : SerializeField] public List<Item> Items { get; private set; }
    [field : SerializeField] public List<Equipment> Equipments { get; private set; }
}
