using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetController : MonoBehaviour
{
    public Creature Player { get; private set; }
    public List<Creature> Friends { get; private set; }
    public List<Creature> Enemies { get; private set; }
    public List<Creature> Targets { get; private set; }

    public void SetBase()
    {
        Player = null;
        Friends = new List<Creature>();
        Enemies = new List<Creature>();
        Targets = new List<Creature>();
    }
    
    public void AddPlayer(Creature player) { Player = player; }

    public void AddFriend(Creature friend) { Friends.Add(friend); }

    public void AddEnemy(Creature enemy) { Enemies.Add(enemy); }

    public bool IsPlayer(Creature creature) { return Player == creature; }

    public bool IsFriend(Creature creature) { return Friends.Contains(creature); }

    public bool IsEnemy(Creature creature) { return Enemies.Contains(creature); }

    public void Remove(Creature creature)
    {
        if(IsPlayer(creature)) { Player = null; }
        else if(IsFriend(creature)) { Friends.Remove(creature); }
        else if(IsEnemy(creature)) { Enemies.Remove(creature); }
    }

    public List<Creature> FindTargets(Creature creature, TargetType targetType)
    {
        Targets = new List<Creature>();
        switch(targetType)
        {
            case TargetType.None:
                break;
            case TargetType.Self:
                Targets.Add(creature);
                break;
            case TargetType.Friend:
                if(IsEnemy(creature))
                {
                    foreach(Creature enemy in Enemies) { if(enemy != creature) { Targets.Add(enemy); }}
                }
                else
                {
                    Targets.Add(Player);
                    foreach(Creature friend in Friends) { if(friend != creature) { Targets.Add(friend); }}
                }
                break;
            case TargetType.Enemy:
                if(IsEnemy(creature))
                {
                    Targets.Add(Player);
                    foreach(Creature friend in Friends) { Targets.Add(friend); }
                }
                else
                {
                    foreach(Creature enemy in Enemies) { Targets.Add(enemy); }
                }
                break;
            case TargetType.All:
                Targets.Add(Player);
                foreach(Creature friend in Friends) { Targets.Add(friend); }
                foreach(Creature enemy in Enemies) { Targets.Add(enemy); }
                break;
        }
        return Targets;
    }

    public List<Creature> FindTargets(Creature creature, Spell spell)
    {
        return FindTargets(creature, spell.Base.TargetType);
    }

    public List<Creature> FindTargets(Creature creature, Item item)
    {
        return FindTargets(creature, item.Base.TargetType);
    }

    public void EnableSelection(bool enable)
    {
        foreach(Creature creature in Targets) { creature.UI.SetInteractable(enable); }
    }
}
