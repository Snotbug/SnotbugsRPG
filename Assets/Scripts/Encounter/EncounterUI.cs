using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class EncounterUI : MonoBehaviour//, IPointerEnterHandler, IPointerExitHandler
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



// public class SpellUI : MonoBehaviour
// {   
    

//     public Spell Spell { get; private set; }

//     public void SetUI(Spell spell)
//     {
//         Spell = spell;
//         SetInteractable(false);
//     }

//     public void SetInteractable(bool interactable) { Button.interactable = interactable; }

//     public void OnClick() { EventManager.current.ClickSpell(Spell); }
    
//     public void OnPointerEnter(PointerEventData eventData) { EventManager.current.HoverEnterSpell(Spell); }

//     public void OnPointerExit(PointerEventData eventData) { EventManager.current.HoverExitSpell(); }
// }