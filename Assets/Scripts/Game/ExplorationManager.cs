using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplorationManager : MonoBehaviour
{
    [field : SerializeField] public ExplorationUI UI { get; private set; }
    [field : SerializeField] public EncounterBase RootEncounter { get; private set; }

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

    public void AddSpell(ChoiceData data)
    {
        Spell spell = Instantiate(data.Spell, this.transform.position, Quaternion.identity);
        Player.AddSpell(spell);
    }

    public void RemoveSpell(ChoiceData data)
    {
        Spell spell = Instantiate(data.Spell, this.transform.position, Quaternion.identity);
        Player.AddSpell(spell);
    }
}