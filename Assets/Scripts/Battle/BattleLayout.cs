using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BattleLayout : MonoBehaviour
{
    [field : SerializeField] public Sprite Background { get; private set; }

    [field : SerializeField] public CreatureContainer Player { get; private set; }
    [field : SerializeField] public List<CreatureContainer> Friends { get; private set; }
    [field : SerializeField] public List<CreatureContainer> Enemies { get; private set; }
}