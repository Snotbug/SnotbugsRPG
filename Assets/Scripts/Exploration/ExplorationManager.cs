using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplorationManager : MonoBehaviour
{
    [field : SerializeField] public ExplorationUI UI { get; private set; }
    [field : SerializeField] public Choice ChoicePrefab { get; private set; }

    public static ExplorationManager current;

    public EncounterBase Encounter { get; private set; }
    public Creature Player { get; private set; }

    public List<Choice> Choices { get; private set; }

    public Choice SelectedChoice { get; private set; }
    public Spell SelectedSpell { get; private set; }
    public Item SelectedItem { get; private set; }
    public Equipment SelectedEquipment { get; private set; }
    
    public void OnEnable()
    {
        EventManager.current.onClickChoice += SelectChoice;
        EventManager.current.onClickSpell += SelectSpell;
        EventManager.current.onClickItem += SelectItem;
        EventManager.current.onClickEquipment += SelectEquipment;
    }

    public void OnDisable()
    {
        EventManager.current.onClickChoice -= SelectChoice;
        EventManager.current.onClickSpell -= SelectSpell;
        EventManager.current.onClickItem -= SelectItem;
        EventManager.current.onClickEquipment -= SelectEquipment;
    }

    public void Awake()
    {
        current = this;
    }

    public void LoadEncounter(EncounterBase encounter)
    {
        Encounter = encounter;
        
    }

    public void SelectChoice(Choice choice)
    {
        if(SelectedChoice != null) { return; }
        SelectedChoice = choice;
        StartCoroutine(SelectedChoice.EnactConsequences());
    }

    public void SelectSpell(Spell spell)
    {
        if(SelectedSpell != null) { return; }
        SelectedSpell = spell;
    }

    public void SelectItem(Item item)
    {
        if(SelectedItem != null) { return; }
        SelectedItem = item;
    }

    public void SelectEquipment(Equipment equipment)
    {
        if(SelectedEquipment != null) { return; }
        SelectedEquipment = equipment;
    }
    
    public void EnterExploration(Creature player, EncounterBase encounter)
    {
        Player = player;
        UI.gameObject.SetActive(true);
        SetEncounter(encounter);
        UI.SetUI(Encounter);
    }

    public void ExitExploration()
    {
        UI.SetBase();
        UI.gameObject.SetActive(false);
    }

    public void SetEncounter(EncounterBase encounter)
    {
        Encounter = encounter;

        Choices = new List<Choice>();
        foreach(ChoiceBase choice in Encounter.Choices)
        {
            Choice temp = Instantiate(ChoicePrefab, this.transform.position, Quaternion.identity);
            temp.SetBase(Player, choice);
            Choices.Add(temp);
            if(!temp.CheckRequirements(Player)) { temp.UI.SetInteractable(false); }
            UI.AddChoice(temp);
        }
    }

    public void ModifyStats(ChoiceData data)
    {
        foreach(StatBase statBase in data.Base.Stats)
        {
            Stat stat = data.Owner.FindStat(statBase.Definition);
            stat.Modify(statBase.Current);
            data.Owner.UI.UpdateUI();
        }
        data.OnComplete();
    }

    public void SetStats(ChoiceData data)
    {
        foreach(StatBase statBase in data.Base.Stats)
        {
            Stat stat = Player.FindStat(statBase.Definition);
            stat.Set(statBase.Current);
        }
        data.OnComplete();
    }

    public void AddStatus(ChoiceData data)
    {

    }

    public void RemoveStatus(ChoiceData data)
    {

    }

    public void AddSpell(ChoiceData data)
    {
        Spell spell = Instantiate(data.Base.Spell, this.transform.position, Quaternion.identity);
        Player.AddSpell(spell);
    }

    public void RemoveSpell(ChoiceData data)
    {
        ExplorationManager.current.StartCoroutine(WaitForSpell(() => 
        {
            Debug.Log(ExplorationManager.current.Player == null);
            ExplorationManager.current.Player.RemoveSpell(ExplorationManager.current.SelectedSpell);
            ExplorationManager.current.SelectedChoice = null;
            ExplorationManager.current.SelectedSpell = null;
            data.OnComplete();
        }));
    }

    public void AddItem(ChoiceData data)
    {

    }

    public void RemoveItem(ChoiceData data)
    {

    }

    public void AddEquipment(ChoiceData data)
    {

    }

    public void RemoveEquipment(ChoiceData data)
    {

    }

    public IEnumerator WaitForSpell(Action OnComplete)
    {
        yield return new WaitUntil(() => ExplorationManager.current.SelectedSpell != null);
        OnComplete();
    }

    public IEnumerator WaitForItem(Action OnComplete)
    {
        yield return new WaitUntil(() => ExplorationManager.current.SelectedItem != null);
        OnComplete();
    }

    public IEnumerator WaitForEquipment(Action OnComplete)
    {
        yield return new WaitUntil(() => ExplorationManager.current.SelectedEquipment != null);
        OnComplete();
    }
}