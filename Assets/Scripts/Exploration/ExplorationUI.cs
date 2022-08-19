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
    [field : SerializeField] public ChoiceUI ChoicePrefab { get; private set; }

    public EncounterBase Encounter { get; private set; }
    public List<ChoiceUI> Choices { get; private set; }

    public void SetBase()
    {
        for(int i = Choices.Count - 1; i > 0; i--)
        {
            ChoiceUI temp = Choices[i];
            Choices.Remove(temp);
            Destroy(temp);
        }
    }

    public void SetUI(EncounterBase encounter)
    {
        Encounter = encounter;
        Name.text = Encounter.Name;
        Description.text = Encounter.Description;

        Choices = new List<ChoiceUI>();
        foreach(Choice choice in encounter.Choices)
        {
            ChoiceUI temp = Instantiate(ChoicePrefab, ChoiceLayout.transform.position, Quaternion.identity);
            temp.SetBase(choice);
            Choices.Add(temp);
        }
    }
}