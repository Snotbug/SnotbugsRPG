using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplorationManager : MonoBehaviour
{
    [field : SerializeField] public EncounterBase RootEncounter { get; private set; }

    [field : SerializeField] public ExplorationUI UI { get; private set; }

    public static ExplorationManager current;

    [field : SerializeField] public Choice ChoicePrefab { get; private set; }

    public EncounterBase Encounter { get; private set; }
    public Creature Player { get; private set; }

    public Choice SelectedChoice { get; private set; }

    public Spell SelectedSpell { get; private set; }

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

    public void SelectChoice(Choice choice)
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
            Choice temp = Instantiate(ChoicePrefab, this.transform.position, Quaternion.identity);
            temp.SetBase(choice);
            if(!temp.CheckRequirements(Player)) {}
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

    public void ModifyStats(ChoiceData data)
    {
        foreach(StatBase statBase in data.Base.Stats)
        {
            Stat stat = Player.FindStat(statBase.Definition);
            stat.Modify(statBase.Current);
            stat.ModifyMax(statBase.Max);
        }
        data.OnComplete();
    }

    public void SetStats(ChoiceData data)
    {
        foreach(StatBase statBase in data.Base.Stats)
        {
            Stat stat = Player.FindStat(statBase.Definition);
            stat.Set(statBase.Current);
            stat.SetMax(statBase.Max);
        }
        data.OnComplete();
    }

    public void AddSpell(ChoiceData data)
    {
        Spell spell = Instantiate(data.Base.Spell, this.transform.position, Quaternion.identity);
        Player.AddSpell(spell);
    }

    public void Erase(ChoiceData data)
    {
        StartCoroutine(WaitForSpell(RemoveSpell));
    }

    public IEnumerator WaitForSpell(Action OnComplete)
    {
        yield return new WaitUntil(() => SelectedSpell != null);
        OnComplete();
    }

    public void RemoveSpell()
    {
        Player.RemoveSpell(SelectedSpell);
    }
}