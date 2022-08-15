using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using UnityEngine.Events;

public class BattleManager : MonoBehaviour
{
    [field : SerializeField] public BattleUI UI { get; private set; }
    [field : SerializeField] public TargetController TargetController { get; private set; }
    [field : SerializeField] public TurnController TurnController { get; private set; }
    [field : SerializeField] public EffectController EffectController { get; private set; }

    public Spell SelectedSpell { get; private set; }

    public static BattleManager current;

    public void OnEnable()
    {
        EventManager.current.onClickCreature += SelectCreature;
        EventManager.current.onClickSpell += SelectSpell;
    }

    public void OnDisable()
    {
        EventManager.current.onClickCreature -= SelectCreature;
        EventManager.current.onClickSpell -= SelectSpell;
    }

    public void Awake()
    {
        current = this;

        TurnController.SetBase();
        TargetController.SetBase();
        EffectController.SetBase();

        EnterBattle();
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(1))
        {
            SelectedSpell = null;
            TargetController.EnableSelection(false);
        }
    }

    public void ErrorCheck()
    {
        if(UI.Player.Creature.Health.Current <= 0)
        {
            TargetController.Remove(UI.Player.Creature);
            TurnController.Remove(UI.Player.Creature);
            UI.RemoveCreature(UI.Player.Creature);
        }
        foreach(CreatureSpawner friend in UI.Friends)
        {
            Creature creature = friend.Creature;
            if(creature?.Health.Current <= 0)
            {
                TargetController.Remove(creature);
                TurnController.Remove(creature);
                UI.RemoveCreature(creature);
            }
        }
        foreach(CreatureSpawner enemy in UI.Enemies)
        {
            Creature creature = enemy.Creature;
            if(creature?.Health.Current <= 0)
            {
                TargetController.Remove(creature);
                TurnController.Remove(creature);
                UI.RemoveCreature(creature);
            }
        }
        if(TurnController.ActiveCreature == null) { StartTurn(); }
    }

    public void EnterBattle()
    {
        AddPlayer(UI.Player.CreaturePrefab);
        foreach(CreatureSpawner friend in UI.Friends) { if(friend.CreaturePrefab != null) { AddFriend(friend.CreaturePrefab); }}
        foreach(CreatureSpawner enemy in UI.Enemies) { if(enemy.CreaturePrefab != null) { AddEnemy(enemy.CreaturePrefab); }}

        TurnController.SetTurnOrder();
        TurnController.SortTurnOrder();

        TargetController.EnableSelection(false);
        
        StartTurn();
    }

    public void AddPlayer(Creature creature)
    {
        Creature player = UI.AddPlayer(creature);
        if(player == null) { return; }
        TurnController.Add(player);
        TargetController.AddPlayer(player);
    }

    public void AddFriend(Creature creature)
    {
        Creature friend = UI.AddFriend(creature);
        if(friend == null) { return; }
        TurnController.Add(friend);
        TargetController.AddFriend(friend);
    }

    public void AddEnemy(Creature creature)
    {
        Creature enemy = UI.AddEnemy(creature);
        if(enemy == null) { return; }
        TurnController.Add(enemy);
        TargetController.AddEnemy(enemy);
    }

    public void RemoveCreature(Creature creature)
    {
        UI.RemoveCreature(creature);
        TurnController.Remove(creature);
        TargetController.Remove(creature);
    }

    public void SelectSpell(Spell spell)
    {
        SelectedSpell = spell;
        TargetController.EnableSelection(false);
        if(SelectedSpell.Base.TargetType != TargetType.None)
        {
            TargetController.FindTargets(TurnController.ActiveCreature, spell.Base.TargetType);
            TargetController.EnableSelection(true);
        }
        else { ActivateSpell(); }
    }

    public void SelectCreature(Creature creature)
    {
        if(SelectedSpell != null)
        {
            SelectedSpell.ActivatedEffect.Target = creature;
            TargetController.EnableSelection(false);
            ActivateSpell();
        }
    }

    public void ActivateSpell()
    {
        if(SelectedSpell == null) { return; }
        TurnController.ActiveCreature.PayCost(SelectedSpell);
        SelectedSpell.SetStat(SelectedSpell.Cooldown.Definition.Name, SelectedSpell.Cooldown.Max);
        SelectedSpell.UI.SetInteractable(TurnController.ActiveCreature.CanActivate(SelectedSpell));
        SelectedSpell.Activate();
        EffectController.OnCast.TriggerEffect(TurnController.ActiveCreature, TurnController.ActiveCreature);
        SelectedSpell = null;
        TargetController.EnableSelection(false);
    }

    public void StartTurn()
    {
        TurnController.FindActiveCreature();
        TurnController.ActiveCreature.UI.ShowActiveIndicator(true);
        EffectController.OnTurnStart.TriggerEffect(TurnController.ActiveCreature, TurnController.ActiveCreature);
        MainPhase();
    }

    public void MainPhase()
    {
        TurnController.ActiveCreature.SetStat("Stamina", TurnController.ActiveCreature.Stamina.Max);
        if(TargetController.IsEnemy(TurnController.ActiveCreature))
        {
            List<Spell> Spells = TurnController.ActiveCreature.FindActivatable();
            if(Spells.Count > 0)
            {
                SelectedSpell = TurnController.ActiveCreature.Spells[Random.Range(0, Spells.Count)];
                if(SelectedSpell.Base.TargetType != TargetType.None)
                {
                    TargetController.FindTargets(TurnController.ActiveCreature, SelectedSpell.Base.TargetType);
                    SelectedSpell.ActivatedEffect.Target = TargetController.Targets[Random.Range(0, TargetController.Targets.Count)];
                }
            }
            ActivateSpell();
            EndTurn();
        }
        else
        {
            UI.EnableEndTurn(true);
            TurnController.ActiveCreature.EnableSpells(true);
        }
    }
    
    public void EndTurn()
    {
        EffectController.OnTurnEnd.TriggerEffect(TurnController.ActiveCreature, TurnController.ActiveCreature);
        UI.EnableEndTurn(false);
        TargetController.EnableSelection(false);
        TurnController.ActiveCreature.EnableSpells(false);
        TurnController.ActiveCreature.UI.ShowActiveIndicator(false);
        StartTurn();
    }

    public void ExitBattle()
    {
        UI.gameObject.SetActive(false);
        TurnController.gameObject.SetActive(false);
        TargetController.gameObject.SetActive(false);
    }

    // battle effects

    public void SpawnFriend(Creature source, Creature target, DynamicEffectData data)
    {
        TargetController targetController = BattleManager.current.TargetController;
        if(targetController.IsEnemy(source)) { BattleManager.current.AddEnemy(data.Creature); }
        else { BattleManager.current.AddFriend(data.Creature); }
        data.OnComplete();
    }

    public void SpawnEnemy(Creature source, Creature target, DynamicEffectData data)
    {
        TargetController targetController = BattleManager.current.TargetController;
        if(targetController.IsEnemy(source)) { BattleManager.current.AddFriend(data.Creature); }
        else { BattleManager.current.AddEnemy(data.Creature); }
    }

    public void DeSpawn(Creature source, Creature target, DynamicEffectData data)
    {
        BattleManager.current.RemoveCreature(source);
    }

    public void Damage(Creature source, Creature target, DynamicEffectData data)
    {
        Debug.Log($"source {source.Base.Name} target {target.Base.Name}");
        int damage = source.ApplyScaling(data.Stat.Current, data.Base.Scalings);
        damage = Mathf.Clamp(damage - target.Resistance.Current, 1, target.Health.Max);
        target.ModifyStat(target.Health.Definition.Name, -damage);
        if(data.SendTrigger) { BattleManager.current.EffectController.OnDamage.TriggerEffect(source, target); Debug.Log($"{source.Base.Name} damaged {target.Base.Name}"); }
        if(target.Health.Current <= 0) { BattleManager.current.EffectController.OnDeath.TriggerEffect(source, target); Debug.Log($"{source.Base.Name} killed {target.Base.Name}");}
        data.OnComplete();
    }

    public void Heal(Creature source, Creature target, DynamicEffectData data)
    {
        int heal = source.ApplyScaling(data.Stat.Current, data.Base.Scalings);
        target.ModifyStat(target.Health.Definition.Name, heal);
        if(data.SendTrigger) { BattleManager.current.EffectController.OnHeal.TriggerEffect(source, target); }
        data.OnComplete();
    }

    public void ModifyCreatureStat(Creature source, Creature target, DynamicEffectData data)
    {
        target.ModifyStat(data.Stat.Definition.Name, data.Stat.Current);
        data.OnComplete();
    }

    public void ModifySpellStat(Creature source, Creature target, DynamicEffectData data)
    {
        Spell spell = target.FindSpell(data.Spell);
        if(spell == null) { return; }
        spell.ModifyStat(data.Stat.Definition.Name, data.Stat.Current);
        data.OnComplete();
    }

    public void ModifyStatusStat(Creature source, Creature target, DynamicEffectData data)
    {
        Status status = target.FindStatus(data.Status);
        if(status == null) { return; }
        status.ModifyStat(data.Stat.Definition.Name, data.Stat.Current);
        data.OnComplete();
    }

    public void Buff(Creature source, Creature target, DynamicEffectData data)
    {
        Stat stat = target.FindStat(data.Stat.Definition);
        int modifier = data.Stat.Current;
        source.ApplyScaling(modifier, data.Base.Scalings);
        target.ModifyStat(stat.Definition.Name, modifier);
        if(data.SendTrigger) { BattleManager.current.EffectController.OnBuff.TriggerEffect(source, target); }
        data.OnComplete();
    }

    public void DeBuff(Creature source, Creature target, DynamicEffectData data)
    {
        Stat stat = target.FindStat(data.Stat.Definition);
        int modifier = data.Stat.Current;
        source.ApplyScaling(modifier, data.Base.Scalings);
        target.ModifyStat(stat.Definition.Name, -modifier);
        if(data.SendTrigger) { BattleManager.current.EffectController.OnDebuff.TriggerEffect(source, target); }
        data.OnComplete();
    }

    public void Afflict(Creature source, Creature target, DynamicEffectData data)
    {
        Status status = target.FindStatus(data.Status);
        int duration = source.ApplyScaling(data.Stat.Current, data.Base.Scalings);
        duration = Mathf.Clamp(duration - target.Resistance.Current, 1, duration);
        if(status == null)
        {
            status = Instantiate(data.Status, target.transform.position, Quaternion.identity);
            status.SetBase(target);
            status.SetStat(status.Duration.Definition.Name, duration);
            target.AddStatus(status);
            Debug.Log(status.Duration.Current);
            if(data.SendTrigger) { BattleManager.current.EffectController.OnAfflict.TriggerEffect(source, target); }
        }
        else { status.ModifyStat(status.Duration.Definition.Name, duration); }
        data.OnComplete();
    }

    public void Cure(Creature source, Creature target, DynamicEffectData data)
    {
        Status status = target.FindStatus(data.Status);
        if(status == null) { return; }
        target.RemoveStatus(status);
        if(data.SendTrigger) { BattleManager.current.EffectController.OnCure.TriggerEffect(source, target); }
        data.OnComplete();
    }
}