using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ChoiceUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [field : SerializeField] public TMP_Text Name { get; private set; }
    [field : SerializeField] public Button Button { get; private set; }

    public ChoiceBase Choice { get; private set; }

    public void SetBase(ChoiceBase choice)
    {
        Choice = choice;
        UpdateUI();
    }

    public void UpdateUI()
    {
        Name.text = Choice.Name;
    }

    public void SetInteractable(bool interactable) { if(Button == null) { return; } Button.interactable = interactable; }

    public void OnClick() { EventManager.current.ClickChoice(Choice); }
    
    public void OnPointerEnter(PointerEventData eventData) { EventManager.current.HoverEnterChoice(Choice); }

    public void OnPointerExit(PointerEventData eventData) { EventManager.current.HoverExitChoice(); }
}