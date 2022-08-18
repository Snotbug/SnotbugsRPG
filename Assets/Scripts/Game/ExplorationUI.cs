using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using TMPro;

public class ExplorationUI : MonoBehaviour
{
    [field : SerializeField] public TMP_Text Name { get; private set; }
    [field : SerializeField] public TMP_Text Description { get; private set; }
    [field : SerializeField] public Image Background { get; private set; }
    [field : SerializeField] public VerticalLayoutGroup ChoiceLayout { get; private set; }

    public List<ChoiceUI> Choices { get; private set; }

    // public void SetUI(Encounter encounter)
    // {

    // }
}

[System.Serializable]
public class EncounterChoiceUI
{
    [field : SerializeField] public Image Image { get; private set; }
    [field : SerializeField] public Button Button { get; private set; }
}