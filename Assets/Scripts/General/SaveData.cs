using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Save", menuName = "Save")]
public class SaveData : ScriptableObject
{
    [field : SerializeField] public Creature Player { get; set; }
    [field : SerializeField] public EncounterBase Encounter { get; set; }
}
