using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BattleLayout : MonoBehaviour
{
    [field : SerializeField] public Sprite Background { get; private set; }

    [field : SerializeField] public CreatureContainer DefaultPlayer { get; private set; }
    [field : SerializeField] public List<CreatureContainer> DefaultFriends { get; private set; }
    [field : SerializeField] public List<CreatureContainer> DefaultEnemies { get; private set; }

    public CreatureContainer Player { get; private set; }
    public List<CreatureContainer> Friends { get; private set; }
    public List<CreatureContainer> Enemies { get; private set; }

    public void SetBase()
    {
        Player = Instantiate(DefaultPlayer, DefaultPlayer.transform.position, Quaternion.identity);
        Player.transform.SetParent(this.transform);

        Friends = new List<CreatureContainer>();
        foreach(CreatureContainer friend in DefaultFriends)
        {
            CreatureContainer temp = Instantiate(friend, friend.transform.position, Quaternion.identity);
            temp.transform.SetParent(this.transform);
        }

        Enemies = new List<CreatureContainer>();
        foreach(CreatureContainer enemy in DefaultEnemies)
        {
            CreatureContainer temp = Instantiate(enemy, enemy.transform.position, Quaternion.identity);
            temp.transform.SetParent(this.transform);
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
}