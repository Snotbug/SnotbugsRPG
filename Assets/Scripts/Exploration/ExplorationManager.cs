using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplorationManager : MonoBehaviour
{
    [field : SerializeField] public EncounterBase RootEncounter { get; private set; }

    [field : SerializeField] public ExplorationUI UI { get; private set; }

    public static ExplorationManager current;

    public EncounterBase Encounter { get; private set; }
    public Creature Player { get; private set; }

    public ChoiceBase SelectedChoice { get; private set; }

    public void OnEnable()
    {
        EventManager.current.onClickChoice += SelectChoice;
    }

    public void OnDisable()
    {
        EventManager.current.onClickChoice -= SelectChoice;
    }

    public void Awake()
    {
        current = this;
        Encounter = RootEncounter;
    }

    public void SelectChoice(ChoiceBase choice)
    {
        SelectedChoice = choice;
    }

    public void SelectSpell()
    {

    }

    public void SelectEquipment()
    {

    }
    
    public void EnterExploration(Creature player)
    {
        Player = player;
        UI.gameObject.SetActive(true);
        UI.SetBase();
        UI.SetUI(Encounter);
        foreach(ChoiceBase choice in Encounter.Choices)
        {
            if(!CheckRequirements(choice)) {  }
        }
    }

    public void ExitExploration()
    {
        UI.SetBase();
        UI.gameObject.SetActive(false);
    }

    public void SetEncounter()
    {

    }

    public bool CheckRequirements(ChoiceBase choice)
    {
        if
        (
            !CheckStats(choice) ||
            !CheckStatuses(choice) ||
            !CheckSpells(choice) ||
            !CheckItems(choice) ||
            !CheckEquipments(choice)
        ) { return false; }
        return true;
    }

    public bool CheckStats(ChoiceBase choice)
    {
        foreach(StatBase requirement in choice.Requirements.Stats)
        {
            Stat stat = Player.FindStat(requirement.Definition);
            if(stat.Current < requirement.Current || stat.Max < requirement.Max) { return false; }
        }
        return true;
    }

    public bool CheckStatuses(ChoiceBase choice)
    {
        Status requirement = choice.Requirements.Status;
        Status status = Player.FindStatus(requirement);
        if(status == null) { return false; }
        return true;
    }

    public bool CheckSpells(ChoiceBase choice)
    {
        Spell requirement = choice.Requirements.Spell;
        Spell spell = Player.FindSpell(requirement);
        if(spell == null) { return false; }
        return true;
    }

    public bool CheckItems(ChoiceBase choice)
    {
        Item requirement = choice.Requirements.Item;
        Item item = Player.FindItem(requirement);
        if(item == null) { return false; }
        return true;
    }

    public bool CheckEquipments(ChoiceBase choice)
    {
        Equipment requirement = choice.Requirements.Equipment;
        Equipment equipment = Player.FindEquipment(requirement);
        if(equipment == null) { return false; }
        return true;
    }

    public void ModifyStats(ChoiceData data)
    {
        foreach(StatBase statBase in data.Stats)
        {
            Stat stat = Player.FindStat(statBase.Definition);
            stat.Modify(statBase.Current);
            stat.ModifyMax(statBase.Max);
        }
    }

    public void SetStats(ChoiceData data)
    {
        foreach(StatBase statBase in data.Stats)
        {
            Stat stat = Player.FindStat(statBase.Definition);
            stat.Set(statBase.Current);
            stat.SetMax(statBase.Max);
        }
    }

    public void AddSpell()
    {
        ChoiceData data = SelectedChoice.Consequence.Data;
        Spell spell = Instantiate(data.Spell, this.transform.position, Quaternion.identity);
        Player.AddSpell(spell);
    }

    public void RemoveSpell()
    {
        // Spell spell = Instantiate(data.Spell, this.transform.position, Quaternion.identity);
        // Player.AddSpell(spell);
    }
}