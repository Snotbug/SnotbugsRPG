using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplorationManager : MonoBehaviour
{
    public Creature Player { get; private set; }
    
    public void EnterExploration(Creature player)
    {
        Player = player;
    }
}
