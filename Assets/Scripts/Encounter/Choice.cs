using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Choice : MonoBehaviour
{
    [field : SerializeField] public ChoiceBase Base { get; private set; }
    [field : SerializeField] public ChoiceUI UI { get; private set; }
}
