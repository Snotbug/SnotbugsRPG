using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnController : MonoBehaviour
{
    public List<Creature> Creatures { get; private set; }
    public List<Creature> TurnOrder { get; private set; }
    public Creature ActiveCreature { get; private set; }

    public void SetBase()
    {
        Creatures = new List<Creature>();
        TurnOrder = new List<Creature>();
    }

    public void Add(Creature creature)
    {
        if(Creatures.Contains(creature)) { return; }
        Creatures.Add(creature);
        TurnOrder.Add(creature);
    }

    public void Remove(Creature creature)
    {
        if(!Creatures.Contains(creature)) { return; }
        if(ActiveCreature == creature) { ActiveCreature = null; }
        Creatures.Remove(creature);
        TurnOrder.Remove(creature);
    }

    public void SetTurnOrder()
    {
        TurnOrder = new List<Creature>();
        foreach(Creature creature in Creatures) { TurnOrder.Add(creature); }
    }
    
    public void SortTurnOrder()
    {
        TurnOrder.Sort((a, b) => a.Speed.Current.CompareTo(b.Speed.Current));
        TurnOrder.Reverse();
    }

    public Creature FindActiveCreature()
    {
        if(TurnOrder.Count <= 0)
        {
            SetTurnOrder();
            SortTurnOrder();
        }

        ActiveCreature = TurnOrder[0];
        TurnOrder.Remove(TurnOrder[0]);
        return ActiveCreature;
    }
}
