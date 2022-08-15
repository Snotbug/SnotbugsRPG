using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class BattleUI : MonoBehaviour
{
    [field : SerializeField] public CreatureSpawner Player { get; private set; }
    [field : SerializeField] public List<CreatureSpawner> Friends { get; private set; }
    [field : SerializeField] public List<CreatureSpawner> Enemies { get; private set; }
    [field : SerializeField] public Button EndTurn { get; private set; }

    [field : SerializeField] public TMP_Text Description { get; private set; }

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

    public CreatureSpawner FindEmptyPlayer()
    {
        if(Player.IsEmpty()) { return Player; }
        return null;
    }

    public Creature AddPlayer(Creature creature)
    {
        CreatureSpawner player = FindEmptyPlayer();
        if(player == null) { return null; }
        else
        {
            player.Spawn(creature);
            return player.Creature;
        }
    }

    public CreatureSpawner FindEmptyFriend()
    {
        foreach(CreatureSpawner friend in Friends) { if(friend.IsEmpty()) { return friend; }}
        return null;
    }

    public Creature AddFriend(Creature creature)
    {
        CreatureSpawner friend = FindEmptyFriend();
        if(friend == null) { return null; }
        else
        {
            friend.Spawn(creature);
            return friend.Creature;
        }
    }

    public CreatureSpawner FindEmptyEnemy()
    {
        foreach(CreatureSpawner enemy in Enemies) { if(enemy.IsEmpty()) { return enemy; }}
        return null;
    }

    public Creature AddEnemy(Creature creature)
    {
        CreatureSpawner enemy = FindEmptyEnemy();
        if(enemy == null) { return null; }
        else
        {
            enemy.Spawn(creature);
            return enemy.Creature;
        }
    }
    
    public void RemoveCreature(Creature creature)
    {
        CreatureSpawner spawner = Player;
        if(spawner.Creature == creature) { Player.Despawn(); }
        
        spawner = Friends.FirstOrDefault(n => n.Creature != null && n.Creature.Equals(creature));
        if(spawner != null) { spawner.Despawn(); }

        spawner = Enemies.FirstOrDefault(n => n.Creature != null && n.Creature.Equals(creature));
        if(spawner != null) { spawner.Despawn(); }
    }

    public void InspectCreature(Creature creature)
    {
        // Debug.Log(creature.Base.Name);
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

    public void EnableEndTurn(bool enable) { if(EndTurn == null) { return; } EndTurn.interactable = enable; }
}
