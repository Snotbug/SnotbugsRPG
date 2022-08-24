using System.Collections.Generic;
using UnityEngine;

public class ExplorationManager : MonoBehaviour
{
    [field : SerializeField] public ExplorationUI UI { get; private set; }
    [field : SerializeField] public Choice ChoicePrefab { get; private set; }

    [field : SerializeField] public SelectionController Selector { get; private set; }

    public static ExplorationManager current;

    public EncounterBase Encounter { get; private set; }
    public Creature Player { get; private set; }

    public List<Choice> Choices { get; private set; }

    public void Awake()
    {
        current = this;
    }

    public void EnterExploration(Creature player, EncounterBase encounter)
    {
        Player = player;
        Encounter = encounter;

        UI.gameObject.SetActive(true);
        UI.SetUI(Encounter);

        Choices = new List<Choice>();
        foreach(ChoiceBase choice in Encounter.Choices)
        {
            Choice temp = Instantiate(ChoicePrefab, this.transform.position, Quaternion.identity);
            temp.SetBase(Player, choice);
            Choices.Add(temp);
            if(!temp.CheckRequirements(Player)) { temp.UI.SetInteractable(false); }
            UI.AddChoice(temp);
        }

        Selector.WaitForChoice = CheckChoice;
    }

    public void ExitExploration()
    {
        for(int i = Choices.Count - 1; i > 0; i--)
        {
            Choice temp = Choices[i];
            Destroy(temp.gameObject);
            Choices.Remove(temp);
        }
        UI.SetBase();
        UI.gameObject.SetActive(false);

        ExplorationManager.current.gameObject.SetActive(false);
    }

    public void CheckChoice()
    {
        Selector.Choice.UI.SetInteractable(false);
        StartCoroutine(Selector.Choice.EnactConsequences(() =>
        {
            UI.UpdateDescription(Selector.Choice.Base.Description);
            if(Selector.Choice.Base.NextEncounter != null)
            {
                ExitExploration();
            }
            ExplorationManager.current.Selector.Choice = null;
        }));
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
            ExplorationManager.current.Selector.WaitForStatus = (() =>
            {
                ExplorationManager.current.Player.RemoveStatus(ExplorationManager.current.Selector.Status);
                data.OnComplete();
                ExplorationManager.current.Selector.Status = null;
                ExplorationManager.current.Selector.WaitForStatus = null;
            });
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
            ExplorationManager.current.Selector.WaitForSpell = (() =>
            {
                ExplorationManager.current.Player.RemoveSpell(ExplorationManager.current.Selector.Spell);
                data.OnComplete();
                ExplorationManager.current.Selector.Spell = null;
                ExplorationManager.current.Selector.WaitForSpell = null;
            });
        }
    }

    public void AddItem(ChoiceData data)
    {

    }

    public void RemoveItem(ChoiceData data)
    {
        if(data.Base.Item != null)
        {
            Item temp = Player.FindItem(data.Base.Item);
            if(temp != null) { Player.RemoveItem(temp); }
            data.OnComplete();
        }
        else
        {
            ExplorationManager.current.Selector.WaitForItem = (() =>
            {
                ExplorationManager.current.Player.RemoveItem(ExplorationManager.current.Selector.Item);
                data.OnComplete();
                ExplorationManager.current.Selector.Item = null;
                ExplorationManager.current.Selector.WaitForItem = null;
            });
        }
    }

    public void AddEquipment(ChoiceData data)
    {

    }

    public void RemoveEquipment(ChoiceData data)
    {
        if(data.Base.Equipment != null)
        {
            Equipment temp = Player.FindEquipment(data.Base.Equipment);
            if(temp != null) { Player.RemoveEquipment(temp); }
            data.OnComplete();
        }
        else
        {
            ExplorationManager.current.Selector.WaitForEquipment = (() =>
            {
                ExplorationManager.current.Player.RemoveEquipment(ExplorationManager.current.Selector.Equipment);
                data.OnComplete();
                ExplorationManager.current.Selector.Equipment = null;
                ExplorationManager.current.Selector.WaitForEquipment = null;
            });
        }
    }
}