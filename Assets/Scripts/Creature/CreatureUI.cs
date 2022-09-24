using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class CreatureUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [field : SerializeField] public TMP_Text Health { get; private set; }
    [field : SerializeField] public TMP_Text Mana { get; private set; }
    [field : SerializeField] public TMP_Text Stamina { get; private set; }

    [field : SerializeField] public TMP_Text Strength { get; private set; }
    [field : SerializeField] public TMP_Text Dexterity { get; private set; }
    [field : SerializeField] public TMP_Text Knowledge { get; private set; }
    [field : SerializeField] public TMP_Text Resistance { get; private set; }    
    [field : SerializeField] public TMP_Text Speed { get; private set; }

    [field : SerializeField] public Button Button { get; private set; }

    [field : SerializeField] public GridLayoutGroup Statuses { get; private set; }
    [field : SerializeField] public GridLayoutGroup Spells { get; private set; }
    [field : SerializeField] public GridLayoutGroup Items { get; private set; }

    [field : SerializeField] public Animator ActiveIndicator { get; private set; }
    [field : SerializeField] public Animator TargetIndicator { get; private set; }
    
    public Creature Creature { get; private set; }

    public void SetUI(Creature creature)
    {
        Creature = creature;
        SetInteractable(false);
        ShowActiveIndicator(false);
        UpdateUI();
    }

    public void UpdateUI()
    {
        Health.text = $"HP: {Creature.Health.Current}";
        Mana.text = $"MP: {Creature.Mana.Current}";
        Stamina.text = $"SP: {Creature.Stamina.Current}";

        Strength.text = $"STR: {Creature.Strength.Current}";
        Dexterity.text = $"DEX: {Creature.Dexterity.Current}";
        Resistance.text = $"RES: {Creature.Resistance.Current}";
        Knowledge.text = $"KNO: {Creature.Knowledge.Current}";
        Speed.text = $"SPD: {Creature.Speed.Current}";
    }

    public void AddSpell(Spell spell)
    {
        spell.UI.transform.SetParent(Spells.transform);
        spell.UI.transform.localScale = Spells.transform.localScale;
    }

    public void AddStatus(Status status)
    {
        status.UI.transform.SetParent(Statuses.transform);
        status.UI.transform.localScale = Statuses.transform.localScale;
    }

    public void OnClick() { EventManager.current.ClickCreature(Creature); }

    public void OnPointerEnter(PointerEventData eventData) { EventManager.current.HoverEnterCreature(Creature); }

    public void OnPointerExit(PointerEventData eventData) { EventManager.current.HoverExitCreature(); }

    public void ShowActiveIndicator(bool show) { if (ActiveIndicator != null) { ActiveIndicator.gameObject.SetActive(show); }}

    public void ShowTargetIndicator(bool show) { if(TargetIndicator != null) { TargetIndicator.gameObject.SetActive(show); }}

    public void SetInteractable(bool interactable)
    {
        if(Button == null) { return; }
        Button.interactable = interactable;
        ShowTargetIndicator(interactable);
    }
}