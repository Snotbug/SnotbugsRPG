using System.Collections.Generic;
using UnityEngine;

public class ExplorationManager : MonoBehaviour
{
    [field : SerializeField] public ExplorationUI UI { get; private set; }
    [field : SerializeField] public SelectionController Selector { get; private set; }
    [field : SerializeField] public Choice ChoicePrefab { get; private set; }

    public static ExplorationManager current;

    public EncounterBase Encounter { get; private set; }
    public Creature Player { get; private set; }

    public List<Choice> Choices { get; private set; }

    public void EnterExploration(Creature player, EncounterBase encounter)
    {
        Player = player;
        Player.gameObject.SetActive(false);
        Encounter = encounter;

        UI.gameObject.SetActive(true);
        UI.SetUI(Encounter);

        Choices = new List<Choice>();
        foreach(ChoiceBase choice in Encounter.Choices)
        {
            Choice temp = Instantiate(ChoicePrefab);
            temp.SetBase(Player, choice);
            Choices.Add(temp);
            if(!temp.CheckRequirements(Player)) { temp.UI.SetInteractable(false); }
            UI.AddChoice(temp);
        }

        foreach(Spell spell in Player.Spells)
        {
            UI.AddSpell(spell);
            spell.UI.gameObject.SetActive(true);
            spell.UI.SetInteractable(true);
        }

        UI.UpdateStats(player);

        Selector.OnSelectChoice = CheckChoice;
    }

    public async void ExitExploration()
    {
        ChoiceBase choice = ExplorationManager.current.Selector.Choice.Base;

        for(int i = Choices.Count - 1; i >= 0; i--)
        {
            Choice temp = Choices[i];
            Destroy(temp.gameObject);
            Choices.Remove(temp);
        }

        foreach(Spell spell in Player.Spells)
        {
            Player.UI.AddSpell(spell);
            spell.UI.gameObject.SetActive(false);
            spell.UI.SetInteractable(false);
        }

        await System.Threading.Tasks.Task.Delay(2000);

        UI.SetBase();
        UI.gameObject.SetActive(false);
        Player.gameObject.SetActive(true);
        EventManager.current.ExitExploration(Player, choice.NextEncounter, choice.BattleLayout);
    }

    public void CheckChoice()
    {
        // Selector.Choice.UI.SetInteractable(false);
        Selector.Choice.gameObject.SetActive(false);
        if(Selector.Choice.Base.PreDescription != "") { UI.UpdateDescription(Selector.Choice.Base.PreDescription); }
        StartCoroutine(Selector.Choice.EnactConsequences(() =>
        {
            UI.UpdateDescription(Selector.Choice.Base.Description);
            if(Selector.Choice.Base.NextEncounter != null)
            {
                ExitExploration();
            }
            ExplorationManager.current.Selector.Choice = null;
        }));
        foreach(Choice choice in Choices)
        {
            if(!choice.CheckRequirements(Player)) { choice.UI.SetInteractable(true); }
        }

        UI.UpdateStats(Player);
    }
}