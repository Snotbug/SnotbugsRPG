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

    public List<ChoiceUI> Choices { get; private set; }

    public void SetBase()
    {
        Name.text = "";
        Description.text = "";

        for(int i = Choices.Count - 1; i > 0; i--)
        {
            ChoiceUI temp = Choices[i];
            Choices.Remove(temp);
            Destroy(temp);
        }
    }

    public void SetUI(EncounterBase encounter)
    {
        Name.text = encounter.Name;
        Description.text = encounter.Description;

        Choices = new List<ChoiceUI>();
    }

    public void AddChoice(Choice choice)
    {
        choice.transform.SetParent(ChoiceLayout.transform);
        choice.transform.localScale = ChoiceLayout.transform.localScale;
        // Choices.Add(ui);
    }
}