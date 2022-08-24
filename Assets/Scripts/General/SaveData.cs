using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Save", menuName = "Save")]
public class SaveData : ScriptableObject
{
    public Creature Player { get; set; }
    public EncounterBase Encounter { get; set; }
}
