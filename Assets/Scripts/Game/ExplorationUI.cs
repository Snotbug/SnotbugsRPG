using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ExplorationUI : MonoBehaviour//, IPointerEnterHandler, IPointerExitHandler
{
    [field : SerializeField] public Image Image { get; private set; }
    [field : SerializeField] public Button Button { get; private set; }

    public void SetUI(Encounter encounter)
    {

    }
}

[System.Serializable]
public class EncounterChoiceUI
{
    [field : SerializeField] public Image Image { get; private set; }
    [field : SerializeField] public Button Button { get; private set; }
}