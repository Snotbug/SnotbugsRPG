using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class ExplorationManager : MonoBehaviour
{
    [field : SerializeField] public List<Spell> Spells { get; private set; }
    [field : SerializeField] public List<Equipment> Equipments { get; private set; }

    public Creature Player { get; private set; }
    
    public void EnterExploration(Creature player)
    {
        Player = player;
    }
}
