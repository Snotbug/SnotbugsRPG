using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Encounter", menuName = "Encounter")]
public class EncounterBase : ScriptableObject
{
    [field : SerializeField] public string Name { get; private set; }
    [field : SerializeField] public string Description { get; private set; }

    [field : SerializeField] public List<EncounterChoice> Choices { get; private set; }
}