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

        TurnController.SetTurnOrder();
        TurnController.SortTurnOrder();

        TargetController.EnableSelection(false);

        StartTurn();
    }

    public void ExitBattle()
    {
        TurnController.SetBase();
        TargetController.SetBase();
        EffectController.SetBase();
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
        TurnController.FindActiveCreature();
        TurnController.ActiveCreature.UI.ShowActiveIndicator(true);
        EffectController.OnTurnStart.TriggerEffect(TurnController.ActiveCreature, TurnController.ActiveCreature);
        Debug.Log("waiting for effects");
        EffectController.WaitForEffect = (() =>
        {
            CheckError();
            if(TurnController.ActiveCreature == null) { StartTurn(); }
            else { MainPhase(); }
        });
    }

    public void WaitForAction()
    {
        Selector.WaitForSpell = WaitForTarget;
        Selector.WaitForItem = WaitForTarget;

        UI.EnableEndTurn(true);
        TurnController.ActiveCreature.EnableSpells(true);
    }

    public void WaitForTarget()
    {
        Selector.WaitForSpell = null;
        Selector.WaitForItem = null;

        UI.EnableEndTurn(false);
        TurnController.ActiveCreature.EnableSpells(false);
        TargetController.FindTargets(TurnController.ActiveCreature, Selector.Spell);
        if(TargetController.Targets.Count > 0)
        {
            Selector.WaitForCreature = WaitForEffects;
            TargetController.EnableSelection(true);
        }
        else { WaitForEffects(); }
    }

    public void WaitForEffects()
    {

    }

    public void MainPhase()
    {
        TurnController.ActiveCreature.SetStat("Stamina", TurnController.ActiveCreature.Stamina.Max);
        if(TargetController.IsEnemy(TurnController.ActiveCreature)) { EnemyTurn(); }
        else { PlayerTurn();}
    }

    public void EnemyTurn()
    {
        List<Spell> Spells = TurnController.ActiveCreature.FindActivatable();
        if(Spells.Count > 0)
        {
            Selector.SelectSpell(TurnController.ActiveCreature.Spells[Random.Range(0, Spells.Count)]);
            List<Creature> targets = TargetController.FindTargets(TurnController.ActiveCreature, Selector.Spell);
            if(targets.Count > 0)
            {
                Selector.Spell.ActivatedEffect.Target = targets[Random.Range(0, TargetController.Targets.Count)];
            }
        }
        
        Selector.Spell.ActivateQueued();
        EffectController.OnCast.TriggerEffect(TurnController.ActiveCreature, TurnController.ActiveCreature);
        // StartCoroutine(EffectController.AwaitEffects(() =>
        // {
        //     CheckError();
        //     if(TurnController.ActiveCreature == null) { StartTurn(); }
        //     else { EndTurn(); }
        // }));
    }

    public void PlayerTurn()
    {
        Debug.Log($"player turn");
        UI.EnableEndTurn(true);
        TurnController.ActiveCreature.EnableSpells(true);
        
        // StartCoroutine(EffectController.AwaitEffects(() =>
        // {
        //     CheckError();
        //     if(TurnController.ActiveCreature == null) { StartTurn(); }
        //     else { EndTurn(); }
        // }));
    }
    
    public void EndTurn()
    {
        TurnController.ActiveCreature.EnableSpells(false);
        TurnController.ActiveCreature.UI.ShowActiveIndicator(false);

        TargetController.EnableSelection(false);

        UI.EnableEndTurn(false);

        EffectController.OnTurnEnd.TriggerEffect(TurnController.ActiveCreature, TurnController.ActiveCreature);
        // StartCoroutine(EffectController.WaitForEffects(() =>
        // {
        //     StartTurn();
        // }));
    }

    public void CheckError()
    {
        Creature player = Layout.Player.Creature;
        if(player.Health.Current <= 0)
        {
            TurnController.Remove(player);
            TargetController.Remove(player);
            Layout.Player.Remove();
            Destroy(player.gameObject);
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
                Destroy(enemy.gameObject);
            }
        }
        if(TargetController.Enemies.Count <= 0)
        {
            Debug.Log("you won");
        }
    }
}