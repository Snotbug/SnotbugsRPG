using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class EquipmentUI : MonoBehaviour
{
    [field : SerializeField] public Image Image { get; private set; }
    [field : SerializeField] public Button Button { get; private set; }

    public Equipment Equipment { get; private set; }

    public void SetUI(Equipment equipment)
    {
        Equipment = equipment;
        SetInteractable(false);
    }

    public void SetInteractable(bool interactable) { if(Button == null) { return; } Button.interactable = interactable; }

    public void OnClick() { EventManager.current.ClickEquipment(Equipment); }
    
    public void OnPointerEnter(PointerEventData eventData) { EventManager.current.HoverEnterEquipment(Equipment); }

    public void OnPointerExit(PointerEventData eventData) { EventManager.current.HoverExitEquipment(); }
}
