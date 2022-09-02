using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using TMPro;

public class ExplorationUI : MonoBehaviour
{
    [field : SerializeField] public TMP_Text Name { get; private set; }
    [field : SerializeField] public TMP_Text Description { get; private set; }
    [field : SerializeField] public SpriteRenderer Background { get; private set; }
    [field : SerializeField] public VerticalLayoutGroup ChoiceLayout { get; private set; }

    public void SetBase()
    {
        Name.text = "";
        Description.text = "";
    }

    public void SetUI(EncounterBase encounter)
    {
        Name.text = encounter.Name;
        Description.text = encounter.Description;
    }

    public void UpdateDescription(string description)
    {
        Description.text = $"{description}";
    }

    public void AddChoice(Choice choice)
    {
        choice.transform.SetParent(ChoiceLayout.transform);
        choice.transform.localScale = ChoiceLayout.transform.localScale;
    }
}