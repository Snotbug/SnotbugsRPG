using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ItemUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{   
    [field : SerializeField] public Image Image { get; private set; }
    [field : SerializeField] public Button Button { get; private set; }

    public Item Item { get; private set; }

    public void SetUI(Item item)
    {
        Item = item;
        SetInteractable(false);
    }

    public void SetInteractable(bool interactable) { if(Button == null) { return; } Button.interactable = interactable; }

    public void OnClick() { EventManager.current.ClickItem(Item); }
    
    public void OnPointerEnter(PointerEventData eventData) { EventManager.current.HoverEnterItem(Item); }

    public void OnPointerExit(PointerEventData eventData) { EventManager.current.HoverExitItem(); }
}