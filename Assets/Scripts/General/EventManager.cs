using System;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager current;
    public void Awake() { current = this; }

    public event Action<Creature, EncounterBase> onEnterExploration;
    public void EnterExploration(Creature player, EncounterBase encounter) { if(onEnterExploration != null) { onEnterExploration(player, encounter); }}

    public event Action<Creature, EncounterBase, BattleLayout> onExitExploration;
    public void ExitExploration(Creature player, EncounterBase encounter, BattleLayout layout) { if(onExitExploration != null) { onExitExploration(player, encounter, layout); }}

    public event Action<Creature, BattleLayout> onEnterBattle;
    public void EnterBattle(Creature player, BattleLayout battleEncounter) { if(onEnterBattle != null) { onEnterBattle(player, battleEncounter); }}

    public event Action<Choice> onClickChoice;
    public void ClickChoice(Choice choice) { if(onClickChoice != null) { onClickChoice(choice); }}

    public event Action<Choice> onHoverEnterChoice;
    public void HoverEnterChoice(Choice choice) { if(onHoverEnterChoice != null) { onHoverEnterChoice(choice); }}

    public event Action onHoverExitChoice;
    public void HoverExitChoice() { if(onHoverExitChoice != null) { onHoverExitChoice(); }}

    public event Action<Creature> onHoverEnterCreature;
    public void HoverEnterCreature(Creature creature) { if(onHoverEnterCreature != null) { onHoverEnterCreature(creature); }}

    public event Action onHoverExitCreature;
    public void HoverExitCreature() { if(onHoverExitCreature != null) { onHoverExitCreature(); }}

    public event Action<Creature> onClickCreature;
    public void ClickCreature(Creature creature) { if(onClickCreature != null) { onClickCreature(creature); }}

    public event Action<Status> onHoverEnterStatus;
    public void HoverEnterStatus(Status status) { if(onHoverEnterStatus != null) { onHoverEnterStatus(status); }}

    public event Action onHoverExitStatus;
    public void HoverExitStatus() { if(onHoverExitStatus != null) { onHoverExitStatus(); }}

    public event Action<Status> onClickStatus;
    public void ClickStatus(Status status) { if(onClickStatus != null) { onClickStatus(status); }}

    public event Action<Spell> onClickSpell;
    public void ClickSpell(Spell spell) { if(onClickSpell != null) { onClickSpell(spell); }}

    public event Action<Spell> onHoverEnterSpell;
    public void HoverEnterSpell(Spell spell) { if(onHoverEnterSpell != null) { onHoverEnterSpell(spell); }}

    public event Action onHoverExitSpell;
    public void HoverExitSpell() { if(onHoverExitSpell != null) { onHoverExitSpell(); }}

    public event Action<Item> onClickItem;
    public void ClickItem(Item item) { if(onClickItem != null) { onClickItem(item); }}

    public event Action<Item> onHoverEnterItem;
    public void HoverEnterItem(Item item) { if(onHoverEnterItem != null) { onHoverEnterItem(item); }}

    public event Action onHoverExitItem;
    public void HoverExitItem() { if(onHoverExitItem != null) { onHoverExitItem(); }}

    public event Action<Equipment> onClickEquipment;
    public void ClickEquipment(Equipment equipment) { if(onClickEquipment != null) { onClickEquipment(equipment); }}

    public event Action<Equipment> onHoverEnterEquipment;
    public void HoverEnterEquipment(Equipment equipment) { if(onHoverEnterEquipment != null) { onHoverEnterEquipment(equipment); }}

    public event Action onHoverExitEquipment;
    public void HoverExitEquipment() { if(onHoverExitEquipment != null) { onHoverExitEquipment(); }}
}
