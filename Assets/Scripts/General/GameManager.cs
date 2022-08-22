using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [field : SerializeField] public ExplorationManager ExplorationManager { get; private set; }
    [field : SerializeField] public BattleManager BattleManager { get; private set; }


    [field : SerializeField] public Creature Player { get; private set; }
    [field : SerializeField] public EncounterBase RootEncounter { get; private set; }
    [field : SerializeField] public BattleLayout Layout { get; private set; }

    public static GameManager current;

    public void Awake()
    {
        current = this;
    }

    public Creature player { get; private set; }

    private void Start()
    {
        player = Instantiate(Player, this.transform.position, Quaternion.identity);
        player.SetBase();
        foreach(Spell spell in player.Spells)
        {
            spell.UI.SetInteractable(true);
        }
        ExplorationManager.EnterExploration(player, RootEncounter);
        // BattleManager.EnterBattle(player, Layout);
    }
}

public enum GameState
{
    Exploration,
    Battle
}