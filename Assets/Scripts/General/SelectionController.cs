using System;
using UnityEngine;

public class SelectionController : MonoBehaviour
{
    public void OnEnable()
    {
        EventManager.current.onClickCreature += SelectCreature;
        EventManager.current.onClickStatus += SelectStatus;
        EventManager.current.onClickSpell += SelectSpell;
        EventManager.current.onClickChoice += SelectChoice;
    }

    public void OnDisable()
    {
        EventManager.current.onClickCreature -= SelectCreature;
        EventManager.current.onClickStatus -= SelectStatus;
        EventManager.current.onClickSpell -= SelectSpell;
        EventManager.current.onClickChoice -= SelectChoice;
    }

    public Action OnSelectCreature { get; set; }
    public Action OnSelectStatus { get; set; }
    public Action OnSelectSpell { get; set; }
    public Action OnSelectChoice { get; set; }

    public Creature Creature { get; set; }
    public Status Status { get; set; }
    public Spell Spell { get; set; }
    public Choice Choice { get; set; }

    public void SelectCreature(Creature creature)
    {
        if(OnSelectCreature == null || Creature != null) { return; }
        Creature = creature;
        OnSelectCreature();
    }

    public void SelectStatus(Status status)
    {
        if(OnSelectStatus == null || Status != null) { return; }
        Status = status;
        OnSelectStatus();
    }

    public void SelectSpell(Spell spell)
    {
        if(OnSelectSpell == null || Spell != null) { return; }
        Spell = spell;
        OnSelectSpell();
    }

    public void SelectChoice(Choice choice)
    {
        if(OnSelectChoice == null || Choice != null) { return; }
        Choice = choice;
        OnSelectChoice();
    }

    public void StopWaiting()
    {
        OnSelectCreature = null;
        OnSelectStatus = null;
        OnSelectSpell = null;
        OnSelectChoice = null;
    }

    public void ClearSelection()
    {
        Creature = null;
        Status = null;
        Spell = null;
        Choice = null;
    }
}
