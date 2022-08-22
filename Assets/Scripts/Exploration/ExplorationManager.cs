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
    public Status SelectedStatus { get; private set; }
    public Spell SelectedSpell { get; private set; }
    public Item SelectedItem { get; private set; }
    public Equipment SelectedEquipment { get; private set; }
    
    public void OnEnable()
    {
        EventManager.current.onClickChoice += SelectChoice;
        EventManager.current.onClickStatus += SelectStatus;
        EventManager.current.onClickSpell += SelectSpell;
        EventManager.current.onClickItem += SelectItem;
        EventManager.current.onClickEquipment += SelectEquipment;
    }

    public void OnDisable()
    {
        EventManager.current.onClickChoice -= SelectChoice;
        EventManager.current.onClickStatus -= SelectStatus;
        EventManager.current.onClickSpell -= SelectSpell;
        EventManager.current.onClickItem -= SelectItem;
        EventManager.current.onClickEquipment -= SelectEquipment;
    }

    public void Awake()
    {
        current = this;
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

        ExplorationManager.current.gameObject.SetActive(false);
    }

    public void SelectChoice(Choice choice)
    {
        if(SelectedChoice != null) { return; }
        SelectedChoice = choice;
        SelectedChoice.UI.SetInteractable(false);
        StartCoroutine(SelectedChoice.EnactConsequences(() =>
        {
            UI.UpdateDescription(choice.Base.Description);
            if(SelectedChoice.Base.NextEncounter != null)
            {
                ExitExploration();
                EnterExploration(ExplorationManager.current.Player, SelectedChoice.Base.NextEncounter);
            }
            ExplorationManager.current.SelectedChoice = null;
        }));
    }

    public void SelectStatus(Status status)
    {
        if(SelectedStatus != null) { return; }
        SelectedStatus = status;
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
        Status temp = Instantiate(data.Base.Status);
        temp.SetBase(Player);
        Player.AddStatus(temp);
        data.OnComplete();
    }

    public void RemoveStatus(ChoiceData data)
    {
        if(data.Base.Status != null)
        {
            Status temp = Player.FindStatus(data.Base.Status);
            if(temp != null) { Player.RemoveStatus(temp); }
            data.OnComplete();
        }
        else
        {
            ExplorationManager.current.StartCoroutine(WaitForStatus(() => 
            {
                ExplorationManager.current.Player.RemoveStatus(ExplorationManager.current.SelectedStatus);
                ExplorationManager.current.SelectedStatus = null;
                data.OnComplete();
            }));
        }
    }

    public void AddSpell(ChoiceData data)
    {
        Spell temp = Instantiate(data.Base.Spell);
        ExplorationManager.current.Player.AddSpell(temp);
        data.OnComplete();
    }

    public void RemoveSpell(ChoiceData data)
    {
        if(data.Base.Spell != null)
        {
            Spell temp = Player.FindSpell(data.Base.Spell);
            if(temp != null) { Player.RemoveSpell(temp); }
            data.OnComplete();
        }
        else
        {
            ExplorationManager.current.StartCoroutine(WaitForSpell(() => 
            {
                ExplorationManager.current.Player.RemoveSpell(ExplorationManager.current.SelectedSpell);
                ExplorationManager.current.SelectedSpell = null;
                data.OnComplete();
            }));
        }
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

    public IEnumerator WaitForStatus(Action OnComplete)
    {
        yield return new WaitUntil(() => ExplorationManager.current.SelectedStatus != null);
        OnComplete();
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