using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [field : SerializeField] public ExplorationManager ExplorationManager { get; private set; }
    [field : SerializeField] public BattleManager BattleManager { get; private set; }

    [field : SerializeField] public Creature DefaultPlayer { get; private set; }
    [field : SerializeField] public EncounterBase DefaultEncounter { get; private set; }
    
    [field : SerializeField] public SaveData Save { get; private set; }

    public Creature Player { get; private set; }
    public EncounterBase Encounter { get; private set; }

    public static GameManager current;

    public void Awake()
    {
        current = this;
    }

    public void OnEnable()
    {
        // EventManager.current.onEnterBattle += 
    }

    private void Start()
    {
        
        Player.SetBase();
        ExplorationManager.EnterExploration(Player, Save.Encounter);
        // BattleManager.EnterBattle(player, Layout);
    }

    public void UpdateState(GameState state)
    {
        switch(state)
        {
            case GameState.Exploration:
                break;
            case GameState.Battle:
                break;
            default:
                break;
        }
    }

    public void EnterBattle(Creature player, BattleLayout layout)
    {

    }

    public void EnterExploration(Creature player, EncounterBase encounter)
    {

    }

    public void LoadGame()
    {
        if(Save.Player == null)
        {
            Player = Instantiate(DefaultPlayer, this.transform.position, Quaternion.identity);
            Player.SetBase();
            Encounter = DefaultEncounter;
        }
        else
        {
            Player = Instantiate(Save.Player, this.transform.position, Quaternion.identity);
            Encounter = Save.Encounter;
        }
    }

    public void SaveGame()
    {
        Save.Player = Player;
        Save.Encounter = Encounter;
    }
}

public enum GameState
{
    Exploration,
    Battle
}