using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [field : SerializeField] public Creature Player { get; private set; }
    [field : SerializeField] public ExplorationManager ExplorationManager { get; private set; }
    [field : SerializeField] public BattleManager BattleManager { get; private set; }

    public Creature player;

    private void Start()
    {
        player = Instantiate(Player, this.transform.position, Quaternion.identity);
        ExplorationManager.EnterExploration(player);
    }
}
