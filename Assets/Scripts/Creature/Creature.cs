using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Creature : MonoBehaviour
{
    [field: SerializeField] public CreatureData data; 
    [field : SerializeField] public CreatureUI UI { get; private set; }
    [field : SerializeField] public CreatureAnimator Animator { get; private set; }

    private StatBlock _statBlock;

    public StatBlock statBlock // this is intended to create a new component if the creature is does not already have a block. Since the Statblock will pull values from save data, starting with a component will be for testing. 
    {
        get
        {
            if (_statBlock == null)
            {
                if (Application.isPlaying)
                {
                    StatBlock local = gameObject.GetComponent<StatBlock>();
                    if (local == null)
                    {
                        local = gameObject.AddComponent<StatBlock>();
                    }
                    _statBlock = local;
                }
            }
            return _statBlock;
        }
    } 
    
    public List<Status> Statuses { get; private set; }
    public List<Spell> Spells { get; private set; }
    public List<Item> Items { get; private set; }
    public Dictionary<EquipmentType, Equipment> Equipments { get; private set; }

    public void SetBase()
    {
    
        /*
        Statuses = new List<Status>();
        foreach(Status status in Base.Statuses)
        {
            Status temp = Instantiate(status, this.transform.position, Quaternion.identity);
            temp.transform.SetParent(this.transform);
            temp.SetBase(this);
            AddStatus(temp);
        }

        Spells = new List<Spell>();
        foreach(Spell spell in Base.Spells)
        {
            Spell temp = Instantiate(spell, this.transform.position, Quaternion.identity);
            temp.transform.SetParent(this.transform);
            temp.SetBase(this);
            AddSpell(temp);
        }

        Items = new List<Item>();
        foreach(Item item in Base.Items)
        {
            Item temp = Instantiate(item, this.transform.position, Quaternion.identity);
            temp.transform.SetParent(this.transform);
            temp.SetBase(this);
            AddItem(temp);
        }

        Equipments = new Dictionary<EquipmentType, Equipment>();
        foreach(EquipmentType type in Enum.GetValues(typeof(EquipmentType))) { Equipments.Add(type, null); }
        foreach(Equipment equipment in Base.Equipments)
        {
            Equipment temp = Instantiate(equipment, this.transform.position, Quaternion.identity);
            temp.transform.SetParent(this.transform);
            temp.SetBase(this);
            AddEquipment(equipment);
        }
        */

        UI.SetUI(this);
    }

    public void OnDestroy()
    {
        if(UI != null) { Destroy(UI.gameObject); }
        if(Animator != null) { Destroy(Animator?.gameObject); }
    }

   
    public Status FindStatus(Status status)
    {
        return (Status)Statuses.FirstOrDefault(n => n.Base.Name.Equals(status.Base.Name));
    }

    public void AddStatus(Status status)
    {
        if(FindStatus(status) != null) { return; }
        Statuses.Add(status);
        status.transform.SetParent(this.transform);

        if(UI != null) { UI.AddStatus(status); }
    }

    public void RemoveStatus(Status status)
    {
        Status foundStatus = FindStatus(status);
        if(foundStatus == null) { return; }
        Destroy(foundStatus.gameObject);
        Statuses.Remove(foundStatus);
    }

    public Spell FindSpell(Spell spell)
    {
        return (Spell)Spells.FirstOrDefault(n => n.data.Name.Equals(spell.data.Name));
    }
/*
    public bool CanActivate(Spell spell)
    {
        if
        (
            spell.Cooldown.Current > 0
        )
        { return false; }
        foreach(StatBase cost in spell.Base.Costs)
        {
            if(FindStat(cost.Definition).Current < cost.Current) { return false; }
        }
        return true;
    }

    public void PayCost(List<StatBase> costs)
    {
        foreach(StatBase cost in costs) { ModifyStat(cost.Definition.Name, -cost.Current); }
        UI.UpdateUI();
    }

    public void EnableSpells(bool enable)
    {
        if(enable) { foreach(Spell spell in Spells) { spell.UI.SetInteractable(CanActivate(spell)); }}
        else{ foreach(Spell spell in Spells) { spell.UI.SetInteractable(false); }}
    }

    public List<Spell> FindActivatable()
    {
        List<Spell> spells = new List<Spell>();
        foreach(Spell spell in Spells) { if(CanActivate(spell)) { spells.Add(spell); }}
        return spells;
    }
*/
    public void AddSpell(Spell spell)
    {
        if(FindSpell(spell) != null) { return; }
        Spells.Add(spell);
        spell.transform.SetParent(this.transform);

        if(UI != null) { UI.AddSpell(spell); }
    }

    public void RemoveSpell(Spell spell)
    {
        Spell foundSpell = FindSpell(spell);
        if(foundSpell == null) { return; }
        Destroy(foundSpell.gameObject);
        Spells.Remove(foundSpell);
    }

    public Item FindItem(Item item)
    {
        return (Item)Items.FirstOrDefault(n => n.Base.Name.Equals(item.Base.Name));
    }

    public void AddItem(Item item)
    {
        if(FindItem(item) != null) { return; }
        Items.Add(item);

        if(UI != null) { UI.AddItem(item); }
    }

    public void RemoveItem(Item item)
    {
        Item foundItem = FindItem(item);
        if(foundItem == null) { return; }
        Destroy(item.gameObject);
        Items.Remove(item);
    }

    public Equipment FindEquipment(Equipment equipment)
    {
        return Equipments.ContainsKey(equipment.Base.Type) ? Equipments[equipment.Base.Type] : null;
    }

    public void AddEquipment(Equipment equipment)
    {
        if(Equipments[equipment.Base.Type] != null) { return; }
        Equipments[equipment.Base.Type] = equipment;
    }

    public void RemoveEquipment(Equipment equipment)
    {
        Equipment foundEquipment = FindEquipment(equipment);
        if(foundEquipment == null) { return; }
        Destroy(foundEquipment.gameObject);
        Equipments[foundEquipment.Base.Type] = null;
    }
}