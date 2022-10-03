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
    [field : SerializeField] public CombatLog Log { get; private set; }

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
        foreach(CreatureContainer friend in Layout.Friends)
        {
            if(friend.Default != null) { AddFriend(friend.Default); }
        }
        foreach(CreatureContainer enemy in Layout.Enemies)
        {
            if(enemy.Default != null)
            {
                Creature temp = Instantiate(enemy.Default, enemy.transform.position, Quaternion.identity);
                temp.SetBase();
                enemy.Add(temp);
                TurnController.Add(temp);
                TargetController.AddEnemy(temp);
                temp.EnableSpells(false);
            }
        }

        TurnController.SetTurnOrder();
        TurnController.SortTurnOrder();

        foreach(Creature creature in TurnController.Creatures)
        {
            creature.UI.ShowTargetIndicator(false);
            creature.UI.ShowActiveIndicator(false);
            creature.UI.SetInteractable(false);
            creature.EnableSpells(false);
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

        foreach(CreatureContainer container in Layout.Friends)
        {
            if(!container.IsEmpty()) { RemoveCreature(container.Creature); }
        }

        EventManager.current.ExitBattle(Layout.Player.Creature);
    }

    public void AddPlayer(Creature creature)
    {
        CreatureContainer container = Layout.FindEmptyPlayer();
        if(container == null) { return; }
        container.Add(creature);
        TurnController.Add(creature);
        TargetController.AddPlayer(creature);
        creature.EnableSpells(false);
        foreach(Spell spell in creature.Spells) { UI.AddSpell(spell); }
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
        temp.EnableSpells(false);
        foreach(Spell spell in temp.Spells) { UI.AddSpell(spell); }
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
        temp.EnableSpells(false);
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
        BattleManager.current.Selector.OnSelectSpell = OnSelectTarget;

        UI.EnableEndTurn(true);
        foreach(Spell spell in TurnController.ActiveCreature.Spells)
        {
            spell.UI.SetInteractable(TurnController.ActiveCreature.CanActivate(spell));
        }
        TurnController.ActiveCreature.EnableSpells(true);
    }

    public void OnSelectTarget()
    {
        BattleManager.current.Selector.StopWaiting();

        UI.EnableEndTurn(false);
        foreach(Spell spell in TurnController.ActiveCreature.Spells)
        {
            spell.UI.SetInteractable(false);
        }
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
        List<Spell> Spells = TurnController.ActiveCreature.FindActivatable();
        for(int i = Spells.Count - 1; i >= 0; i--)
        {
            List<Creature> targets = TargetController.FindTargets(TurnController.ActiveCreature, Spells[i]);
            if(Spells[i].Base.TargetType != TargetType.None && targets.Count <= 0)
            {
                Spells.Remove(Spells[i]);
            }
        }
        if(Spells.Count > 0)
        {
            Spell selectedSpell = Spells[Random.Range(0, Spells.Count)];
            List<Creature> targets = TargetController.FindTargets(TurnController.ActiveCreature, selectedSpell);
            if(targets.Count > 0)
            {
                selectedSpell.ActivatedEffect.Target = targets[Random.Range(0, TargetController.Targets.Count)];
            }

            if(selectedSpell != null)
            {
                BattleManager.current.Print($"{TurnController.ActiveCreature.Base.Name} used {selectedSpell.Base.Name}");
                selectedSpell.ActivateQueued();
            }
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

    public void Print(string s)
    {
        Log.Print(s);
    }
}