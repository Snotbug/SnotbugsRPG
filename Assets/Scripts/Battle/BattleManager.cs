using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [field : SerializeField] public BattleUI UI { get; private set; }
    [field : SerializeField] public TurnController TurnController { get; private set; }
    [field : SerializeField] public TargetController TargetController { get; private set; }
    [field : SerializeField] public EffectController EffectController { get; private set; }
    [field : SerializeField] public SelectionController Selector { get; private set; }

    public BattleLayout Layout { get; private set; }
    
    public static BattleManager current;

    public void Awake()
    {
        TurnController.SetBase();
        TargetController.SetBase();
        EffectController.SetBase();
    }

    public void EnterBattle(Creature player, BattleLayout layout)
    {
        Layout = Instantiate(layout, BattleManager.current.transform.position, Quaternion.identity);
        Layout.transform.SetParent(BattleManager.current.transform);

        AddPlayer(player);
        foreach(CreatureContainer friend in Layout.Friends) { if(friend.Default != null) { AddFriend(friend.Default); }}
        foreach(CreatureContainer enemy in Layout.Enemies) { if(enemy.Default != null) { AddEnemy(enemy.Default); }}

        foreach(Creature creature in TurnController.Creatures)
        {
            creature.UI.ShowTargetIndicator(false);
            creature.UI.ShowActiveIndicator(false);
            creature.EnableSpells(false);
        }

        TurnController.SetTurnOrder();
        TurnController.SortTurnOrder();

        foreach(Creature creature in TurnController.Creatures)
        {
            creature.UI.SetInteractable(false);
        }

        TargetController.EnableSelection(false);

        StartTurn();
    }

    public void ExitBattle()
    {
        foreach(Creature creature in TurnController.Creatures)
        {
            creature.UI.ShowActiveIndicator(false);
            creature.UI.ShowTargetIndicator(false);
        }

        if(Layout.Player.Creature != null) { Layout.Player.Creature.ResetStats(); }

        TurnController.SetBase();
        TargetController.SetBase();
        EffectController.SetBase();
        Selector.ClearSelection();
        Selector.StopWaiting();

        EventManager.current.ExitBattle(Layout.Player.Creature);
    }

    public void AddPlayer(Creature creature)
    {
        CreatureContainer container = Layout.FindEmptyPlayer();
        if(container == null) { return; }
        container.Add(creature);
        TurnController.Add(creature);
        TargetController.AddPlayer(creature);
    }

    public void AddFriend(Creature creature)
    {
        CreatureContainer container = Layout.FindEmptyFriend();
        if(container == null) { return; }
        Creature temp = Instantiate(creature, container.transform.position, Quaternion.identity);
        temp.SetBase();
        container.Add(temp);
        TurnController.Add(temp);
        TargetController.AddFriend(temp);
    }

    public void AddEnemy(Creature creature)
    {
        CreatureContainer container = Layout.FindEmptyEnemy();
        if(container == null) { return; }
        Creature temp = Instantiate(creature, container.transform.position, Quaternion.identity);
        temp.SetBase();
        container.Add(temp);
        TurnController.Add(temp);
        TargetController.AddEnemy(temp);
    }

    public void RemoveCreature(Creature creature)
    {
        CreatureContainer container = Layout.FindContainer(creature);
        container.Remove();
        TurnController.Remove(creature);
        TargetController.Remove(creature);
        Destroy(creature.gameObject);
    }

    public void StartTurn()
    {
        Debug.Log("turn start");
        TurnController.FindActiveCreature();
        TurnController.ActiveCreature.UI.ShowActiveIndicator(true);
        EffectController.OnTurnStart.TriggerEffect(TurnController.ActiveCreature, TurnController.ActiveCreature);
        if(EffectController.Effects.Count <= 0) { MainPhase(); }
        else
        {
            EffectController.OnEffectComplete = (() =>
            {
                if(CheckError())
                {
                    if(TurnController.ActiveCreature == null) { StartTurn(); }
                    else { MainPhase(); }
                }
            });
        }
    }

    public void OnSelectAction()
    {
        Debug.Log("waiting for action");
        BattleManager.current.Selector.OnSelectSpell = OnSelectTarget;
        Debug.Log($"num activatable {TurnController.ActiveCreature.FindActivatable().Count}");

        UI.EnableEndTurn(true);
        TurnController.ActiveCreature.EnableSpells(true);
    }

    public void OnSelectTarget()
    {
        Debug.Log("waiting for target");
        BattleManager.current.Selector.StopWaiting();

        UI.EnableEndTurn(false);
        TurnController.ActiveCreature.EnableSpells(false);
        TargetController.FindTargets(TurnController.ActiveCreature, Selector.Spell);
        if(TargetController.Targets.Count > 0)
        {
            Selector.OnSelectCreature = OnSelectEffects;
            TargetController.EnableSelection(true);
        }
        else { OnSelectEffects(); }
    }

    public void OnSelectEffects()
    {
        Debug.Log("waiting for effects");
        TargetController.EnableSelection(false);
        Selector.Spell.ActivatedEffect.Target = Selector.Creature;

        Selector.Spell.ActivateQueued();
        EffectController.OnCast.TriggerEffect(TurnController.ActiveCreature, TurnController.ActiveCreature);
        EffectController.OnEffectComplete = (() =>
        {
            if(CheckError())
            {
                if(TurnController.ActiveCreature == null) { StartTurn(); }
                else { Selector.ClearSelection(); OnSelectAction(); }
            }
        });
    }

    public void MainPhase()
    {
        TurnController.ActiveCreature.SetStat("Stamina", TurnController.ActiveCreature.Stamina.Max);
        if(TargetController.IsEnemy(TurnController.ActiveCreature)) { EnemyTurn(); }
        else { OnSelectAction();}
    }

    public void EnemyTurn()
    {
        Debug.Log($"{TurnController.ActiveCreature}'s turn");
        List<Spell> Spells = TurnController.ActiveCreature.FindActivatable();
        Spell selectedSpell = null;
        if(Spells.Count > 0)
        {
            selectedSpell = Spells[Random.Range(0, Spells.Count)];
            List<Creature> targets = TargetController.FindTargets(TurnController.ActiveCreature, selectedSpell);
            if(targets.Count > 0)
            {
                selectedSpell.ActivatedEffect.Target = targets[Random.Range(0, TargetController.Targets.Count)];
            }

            if(selectedSpell != null) { Debug.Log($"{TurnController.ActiveCreature.Base.Name} activating selected spell"); selectedSpell.ActivateQueued(); }
            EffectController.OnCast.TriggerEffect(TurnController.ActiveCreature, TurnController.ActiveCreature);

            EffectController.OnEffectComplete = (() =>
            {
                if(CheckError())
                {
                    if(TurnController.ActiveCreature == null) { StartTurn(); }
                    else { EndTurn(); }
                }
            });
        }
        else
        {
            EndTurn();
        }
    }
    
    public void EndTurn()
    {
        Selector.StopWaiting();

        TurnController.ActiveCreature.EnableSpells(false);
        TurnController.ActiveCreature.UI.ShowActiveIndicator(false);

        TargetController.EnableSelection(false);

        UI.EnableEndTurn(false);

        EffectController.OnTurnEnd.TriggerEffect(TurnController.ActiveCreature, TurnController.ActiveCreature);
        StartTurn();
    }

    public bool CheckError()
    {
        Debug.Log("checking errors");
        Creature player = Layout.Player.Creature;
        if(player.Health.Current <= 0)
        {
            TurnController.Remove(player);
            TargetController.Remove(player);
            Layout.Player.Remove();
            Destroy(player.gameObject);
            ExitBattle();
            return false;
        }
        foreach(CreatureContainer container in Layout.Friends)
        {
            Creature friend = container.Creature;
            if(friend?.Health.Current <= 0)
            {
                TurnController.Remove(friend);
                TargetController.Remove(friend);
                container.Remove();
                Destroy(friend.gameObject);
            }
        }
        foreach(CreatureContainer container in Layout.Enemies)
        {
            Creature enemy = container.Creature;
            if(enemy?.Health.Current <= 0)
            {
                TurnController.Remove(enemy);
                TargetController.Remove(enemy);
                container.Remove();
                Debug.Log($"removing {enemy.Base.Name}");
                Debug.Log($"turn controller size: {TurnController.Creatures.Count}");
                Destroy(enemy.gameObject);
            }
        }
        if(TargetController.Enemies.Count <= 0)
        {
            ExitBattle();
            return false;
        }
        return true;
    }
}