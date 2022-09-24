using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleUI : MonoBehaviour
{
    [field : SerializeField] public TMP_Text Description { get; private set; }
    [field : SerializeField] public Button EndTurn { get; private set; }
    [field : SerializeField] public GridLayoutGroup Spells { get; private set; }

    public void OnEnable()
    {
        EventManager.current.onHoverEnterCreature += InspectCreature;
        EventManager.current.onHoverEnterSpell += InspectSpell;
        EventManager.current.onHoverEnterStatus += InspectStatus;
    }

    public void OnDisable()
    {
        EventManager.current.onHoverEnterCreature -= InspectCreature;
        EventManager.current.onHoverEnterSpell -= InspectSpell;
        EventManager.current.onHoverEnterStatus -= InspectStatus;
    }

    public void AddSpell(Spell spell)
    {
        spell.UI.transform.SetParent(Spells.transform);
        spell.UI.transform.localScale = Spells.transform.localScale;
    }

    public void InspectCreature(Creature creature)
    {

    }

    public void InspectSpell(Spell spell)
    {
        Description.text =
        $"Cooldown: {spell.Cooldown.Current} \n {spell.Base.Description}";
    }

    public void InspectStatus(Status status)
    {

    }

    public void EnableEndTurn(bool enable) { if(EndTurn != null) { EndTurn.interactable = enable; }  }
}
