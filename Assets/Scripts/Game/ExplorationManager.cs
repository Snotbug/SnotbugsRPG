using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class ExplorationManager : MonoBehaviour
{
    [field : SerializeField] public List<Spell> Spells { get; private set; }
    [field : SerializeField] public List<Equipment> Equipments { get; private set; }

    public Creature Player { get; private set; }
    public List<Spell> spells { get; private set; }
    public List<Equipment> equipments { get; private set; }

    public void OnEnable()
    {
        EventManager.current.onClickSpell += SelectSpell;
        EventManager.current.onClickEquipment += SelectEquipment;
    }

    public void OnDisable()
    {
        EventManager.current.onClickSpell -= SelectSpell;
        EventManager.current.onClickEquipment -= SelectEquipment;
    }
    
    public void EnterExploration(Creature player)
    {
        Player = player;
        
        spells = new List<Spell>();
        foreach(Spell spell in Spells)
        {
            Spell temp = Instantiate(spell, this.transform.position, Quaternion.identity);
            temp.transform.SetParent(this.transform);
            temp.transform.localScale = this.transform.localScale;
            spells.Add(spell);
        }

        equipments = new List<Equipment>();
        foreach(Equipment equipment in Equipments)
        {
            Equipment temp = Instantiate(equipment, this.transform.position, Quaternion.identity);
            equipments.Add(equipment);
        }
    }

    public void SelectSpell(Spell spell)
    {
        Debug.Log(spell.Base.Name);
    }

    public void SelectEquipment(Equipment equipment)
    {
        Debug.Log(equipment.Base.Name);
    }
}
