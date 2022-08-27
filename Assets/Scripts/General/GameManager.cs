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

    [field : SerializeField] public BattleLayout Layout { get; private set; }

    public Creature Player { get; private set; }
    public EncounterBase Encounter { get; private set; }

    public static GameManager current;

    public void Awake()
    {
        current = this;
        ExplorationManager.current = current.ExplorationManager;
        BattleManager.current = current.BattleManager;
    }

    public void OnEnable()
    {
        EventManager.current.onExitExploration += ExitExploration;
        EventManager.current.onExitBattle += ExitBattle;
    }

    public void OnDisable()
    {
        EventManager.current.onExitExploration -= ExitExploration;
        EventManager.current.onExitBattle += ExitBattle;
    }

    private void Start()
    {
        LoadGame();
        ExplorationManager.current.gameObject.SetActive(true);
        // ExplorationManager.current.EnterExploration(Player, Encounter);
        EnterExploration(Player, Encounter);
    }

    public void EnterExploration(Creature player, EncounterBase encounter)
    {
        Player = player;
        Encounter = encounter;

        ExplorationManager.current.gameObject.SetActive(true);
        ExplorationManager.gameObject.SetActive(true);
        ExplorationManager.current.EnterExploration(Player, Encounter);
    }

    public void ExitExploration(Creature player, EncounterBase encounter, BattleLayout layout)
    {
        ExplorationManager.current.gameObject.SetActive(false);
        if(layout != null) { Encounter = encounter; EnterBattle(player, layout); }
        else { EnterExploration(player, encounter); }
    }

    public void EnterBattle(Creature player, BattleLayout layout)
    {
        Player = player;
        BattleManager.current.gameObject.SetActive(true);
        BattleManager.current.EnterBattle(Player, layout);
    }

    public void ExitBattle(Creature player)
    {
        Player = player;

        if(Player != null) { EnterExploration(Player, Encounter); }
        else { BattleManager.current.gameObject.SetActive(false); Debug.Log("you died"); }
    }

    public void SaveGame()
    {
        Save.Player = Player;
        Save.Encounter = Encounter;
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
}
