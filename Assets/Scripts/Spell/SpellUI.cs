using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class SpellUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{   
    [field : SerializeField] public Image Image { get; private set; }
    [field : SerializeField] public Button Button { get; private set; }

    public Spell Spell { get; private set; }

    public void SetUI(Spell spell)
    {
        Spell = spell;
        SetInteractable(false);
    }

    public void SetInteractable(bool interactable) { if(Button == null) { return; } Button.interactable = interactable; }

    public void OnClick() { EventManager.current.ClickSpell(Spell); }
    
    public void OnPointerEnter(PointerEventData eventData) { EventManager.current.HoverEnterSpell(Spell); }

    public void OnPointerExit(PointerEventData eventData) { EventManager.current.HoverExitSpell(); }
}