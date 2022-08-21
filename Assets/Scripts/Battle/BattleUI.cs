using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class BattleUI : MonoBehaviour
{
    [field : SerializeField] public TMP_Text Description { get; private set; }
    [field : SerializeField] public Button EndTurn { get; private set; }

    public CreatureContainer Player { get; private set; }
    public List<CreatureContainer> Friends { get; private set; }
    public List<CreatureContainer> Enemies { get; private set; }

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

    public void SetBase()
    {
        Player.Remove();
        Destroy(Player.gameObject);

        for(int i = Friends.Count - 1; i > 0; i--)
        {
            Friends[i].Remove();
            CreatureContainer temp = Friends[i];
            Friends.Remove(temp);
            Destroy(temp.gameObject);
        }

        for(int i = Enemies.Count - 1; i > 0; i--)
        {
            Enemies[i].Remove();
            CreatureContainer temp = Enemies[i];
            Enemies.Remove(temp);
            Destroy(temp.gameObject);
        }
    }

    public void SetUI(BattleLayout layout)
    {
        Player = Instantiate(layout.Player, layout.Player.transform.position, Quaternion.identity);

        Friends = new List<CreatureContainer>();
        foreach(CreatureContainer friend in layout.Friends)
        {
            CreatureContainer temp = Instantiate(friend, friend.transform.position, Quaternion.identity);
            Friends.Add(temp);
        }

        Enemies = new List<CreatureContainer>();
        foreach(CreatureContainer enemy in layout.Enemies)
        {
            CreatureContainer temp = Instantiate(enemy, enemy.transform.position, Quaternion.identity);
            Enemies.Add(temp);
        }
    }

    public CreatureContainer FindEmptyPlayer()
    {
        if(Player.IsEmpty()) { return Player; }
        return null;
    }

    public CreatureContainer FindEmptyFriend()
    {
        foreach(CreatureContainer friend in Friends) { if(friend.IsEmpty()) { return friend; }}
        return null;
    }

    public CreatureContainer FindEmptyEnemy()
    {
        foreach(CreatureContainer enemy in Enemies) { if(enemy.IsEmpty()) { return enemy; }}
        return null;
    }

    public CreatureContainer FindContainer(Creature creature)
    {
        CreatureContainer container = Player;

        if(container == creature) { return Player; }
        container = Friends.FirstOrDefault(n => n.Creature != null && n.Creature.Equals(creature));
        if(container != null) { return container; }
        container = Enemies.FirstOrDefault(n => n.Creature != null && n.Creature.Equals(creature));
        if(container != null) { return container; }

        return null;
    }

    public void InspectCreature(Creature creature)
    {

    }

    public void InspectSpell(Spell spell)
    {
        Description.text = spell.Base.Description;
    }

    public void InspectItem(Item item)
    {

    }

    public void InspectStatus(Status status)
    {

    }

    public void InspectEquipment(Status status)
    {

    }

    public void EnableEndTurn(bool enable) { if(EndTurn != null) { EndTurn.interactable = enable; }  }
}
