using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StatusUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [field : SerializeField] public Image Image { get; private set; }

    public Status Status { get; private set; }

    public void SetUI(Status status)
    {
        Status = status;
    }

    public void OnPointerEnter(PointerEventData eventData) { EventManager.current.HoverEnterStatus(Status); }

    public void OnPointerExit(PointerEventData eventData) { EventManager.current.HoverExitStatus(); }
}
