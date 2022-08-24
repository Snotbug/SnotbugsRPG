using System;
using UnityEngine;

public class SelectionController : MonoBehaviour
{
    public void OnEnable()
    {
        EventManager.current.onClickCreature += SelectCreature;
        EventManager.current.onClickStatus += SelectStatus;
        EventManager.current.onClickSpell += SelectSpell;
        EventManager.current.onClickItem += SelectItem;
        EventManager.current.onClickEquipment += SelectEquipment;
        EventManager.current.onClickChoice += SelectChoice;
    }

    public void OnDisable()
    {
        EventManager.current.onClickCreature -= SelectCreature;
        EventManager.current.onClickStatus -= SelectStatus;
        EventManager.current.onClickSpell -= SelectSpell;
        EventManager.current.onClickItem -= SelectItem;
        EventManager.current.onClickEquipment -= SelectEquipment;
        EventManager.current.onClickChoice -= SelectChoice;
    }

    public Action WaitForCreature { get; set; }
    public Action WaitForStatus { get; set; }
    public Action WaitForSpell { get; set; }
    public Action WaitForItem { get; set; }
    public Action WaitForEquipment { get; set; }
    public Action WaitForChoice { get; set; }

    public Creature Creature { get; set; }
    public Status Status { get; set; }
    public Spell Spell { get; set; }
    public Item Item { get; set; }
    public Equipment Equipment { get; set; }
    public Choice Choice { get; set; }

    public void SelectCreature(Creature creature)
    {
        if(WaitForCreature == null || Creature != null) { return; }
        Creature = creature;
        WaitForCreature();
    }

    public void SelectStatus(Status status)
    {
        if(WaitForStatus == null || Status != null) { return; }
        Status = status;
        WaitForStatus();
    }

    public void SelectSpell(Spell spell)
    {
        if(WaitForSpell == null || Spell != null) { return; }
        Spell = spell;
        WaitForSpell();
    }

    public void SelectItem(Item item)
    {
        if(WaitForItem == null || Item != null) { return; }
        Item = item;
        WaitForItem();
    }

    public void SelectEquipment(Equipment equipment)
    {
        if(WaitForEquipment == null || Equipment != null) { return; }
        Equipment = equipment;
        WaitForEquipment();
    }

    public void SelectChoice(Choice choice)
    {
        if(WaitForChoice == null || Choice != null) { return; }
        Choice = choice;
        WaitForChoice();
    }
}
