using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BattleLayout : MonoBehaviour
{
    [field : SerializeField] public Sprite Background { get; private set; }

    [field : SerializeField] public CreatureContainer Player { get; private set; }
    [field : SerializeField] public List<CreatureContainer> Friends { get; private set; }
    [field : SerializeField] public List<CreatureContainer> Enemies { get; private set; }

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
}