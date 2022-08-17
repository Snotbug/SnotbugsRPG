using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplorationManager : MonoBehaviour
{
    [field : SerializeField] public List<Encounter> Encounters { get; private set; }

    public Creature Player { get; private set; }

    public void OnEnable()
    {
    }

    public void OnDisable()
    {
    }
    
    public void EnterExploration(Creature player)
    {
        Player = player;
    }
}
