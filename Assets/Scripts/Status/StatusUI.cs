using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StatusUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [field : SerializeField] public Image Image { get; private set; }
    [field : SerializeField] public Button Button { get; private set; }

    public Status Status { get; private set; }

    public void SetUI(Status status)
    {
        Status = status;
        // Image.sprite = Status.Base.Icon;
    }

    public void SetInteractable(bool interactable) { if(Button != null) { Button.interactable = interactable; }}
    
    public void OnClick() { EventManager.current.ClickStatus(Status); }

    public void OnPointerEnter(PointerEventData eventData) { EventManager.current.HoverEnterStatus(Status); }

    public void OnPointerExit(PointerEventData eventData) { EventManager.current.HoverExitStatus(); }
}
