using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ChoiceUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [field : SerializeField] public TMP_Text Name { get; private set; }
     
    [field : SerializeField] public Button Button { get; private set; }
    [field : SerializeField] public Color32 HighlightColor { get; private set; }
    [field : SerializeField] public Color32 DefaultColor { get; private set; }

    public Choice Choice { get; private set; }

    public void SetUI(Choice choice)
    {
        Choice = choice;
        Name.text = Choice.Base.Name;
    }

    public void SetInteractable(bool interactable)
    {
        if(Button != null) { Button.interactable = interactable; }
        if(!interactable) { Name.fontStyle = FontStyles.Strikethrough; }
        else { Name.fontStyle = FontStyles.Normal; }
    }

    public void OnClick() { EventManager.current.ClickChoice(Choice); }
    
    public void OnPointerEnter(PointerEventData eventData) { if(Button.interactable) { Name.color = HighlightColor; }; EventManager.current.HoverEnterChoice(Choice); }

    public void OnPointerExit(PointerEventData eventData) { Name.color = DefaultColor; EventManager.current.HoverExitChoice(); }
}