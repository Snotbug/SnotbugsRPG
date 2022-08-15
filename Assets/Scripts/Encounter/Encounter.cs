using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Encounter : MonoBehaviour
{
    [field : SerializeField] public EncounterBase Base { get; private set; }
    [field : SerializeField] public EncounterUI UI { get; private set; }
}
