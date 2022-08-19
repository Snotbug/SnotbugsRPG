using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BattleLayout", menuName = "BattleLayout")]
public class BattleLayout : ScriptableObject
{
    [field : SerializeField] public Sprite Background { get; private set; }

    [field : SerializeField] public CreatureContainer Player { get; private set; }
    [field : SerializeField] public List<CreatureContainer> Friends { get; private set; }
    [field : SerializeField] public List<CreatureContainer> Enemies { get; private set; }
}