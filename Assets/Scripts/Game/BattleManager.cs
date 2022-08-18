// using System;
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
        Debug.Log("checking error");
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
        
        StartCoroutine(StartTurn());
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
        else { StartCoroutine(ActivateSpell()); }
    }

    public void SelectCreature(Creature creature)
    {
        if(SelectedSpell != null)
        {
            SelectedSpell.ActivatedEffect.Target = creature;
            TargetController.EnableSelection(false);
            StartCoroutine(ActivateSpell());
        }
    }

    public IEnumerator ActivateSpell()
    {
        if(SelectedSpell == null) { yield return null ; }
        TurnController.ActiveCreature.PayCost(SelectedSpell);
        SelectedSpell.SetStat(SelectedSpell.Cooldown.Definition.Name, SelectedSpell.Cooldown.Max);
        SelectedSpell.UI.SetInteractable(TurnController.ActiveCreature.CanActivate(SelectedSpell));
        SelectedSpell.Activate();
        EffectController.OnCast.TriggerEffect(TurnController.ActiveCreature, TurnController.ActiveCreature);
        SelectedSpell = null;
        TargetController.EnableSelection(false);
        yield return new WaitUntil(() => EffectController.Effects.Count <= 0);
        // ErrorCheck();
    }

    public IEnumerator StartTurn()
    {
        TurnController.FindActiveCreature();
        TurnController.ActiveCreature.UI.ShowActiveIndicator(true);
        EffectController.OnTurnStart.TriggerEffect(TurnController.ActiveCreature, TurnController.ActiveCreature);
        Debug.Log($"{TurnController.ActiveCreature}");
        yield return new WaitUntil(() => EffectController.Effects.Count <= 0);
        Debug.Log("starting main phase");
        StartCoroutine(MainPhase());
    }

    public IEnumerator MainPhase()
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
            yield return new WaitUntil(() => EffectController.Effects.Count <= 0);
            EndTurn();
        }
        else
        {
            Debug.Log("starting player turn");
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
        StartCoroutine(StartTurn());
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

    public async void Damage(Creature source, Creature target, DynamicEffectData data)
    {
        int damage = source.ApplyScaling(data.Stat.Current, data.Base.Scalings);
        damage = Mathf.Clamp(damage - target.Resistance.Current, 1, target.Health.Max);
        bool isDead = target.Health.Current <= 0;
        target.ModifyStat("Health", -damage);
        if(data.SendTrigger) { BattleManager.current.EffectController.OnDamage.TriggerEffect(source, target); Debug.Log($"{source.Base.Name} damaged {target.Base.Name} for {-damage}"); }
        if(!isDead && target.Health.Current <= 0) { BattleManager.current.EffectController.OnDeath.TriggerEffect(source, target); Debug.Log($"{source.Base.Name} killed {target.Base.Name}"); }
        await Task.Delay(100);
        data.OnComplete();
    }

    public async void Heal(Creature source, Creature target, DynamicEffectData data)
    {
        int heal = source.ApplyScaling(data.Stat.Current, data.Base.Scalings);
        target.ModifyStat("Health", heal);
        if(data.SendTrigger) { BattleManager.current.EffectController.OnHeal.TriggerEffect(source, target); }
        await Task.Delay(100);
        data.OnComplete();
    }

    public IEnumerator Wait()
    {
        yield return new WaitForSeconds(5f);
    }

    public async void Buff(Creature source, Creature target, DynamicEffectData data)
    {
        Stat stat = target.FindStat(data.Stat.Definition);
        int modifier = source.ApplyScaling(data.Stat.Current, data.Base.Scalings);
        target.ModifyStat(stat.Definition.Name, modifier);
        if(data.SendTrigger) { BattleManager.current.EffectController.OnBuff.TriggerEffect(source, target); }
        await Task.Delay(100);
        data.OnComplete();
    }

    public async void DeBuff(Creature source, Creature target, DynamicEffectData data)
    {
        Stat stat = target.FindStat(data.Stat.Definition);
        int modifier = source.ApplyScaling(data.Stat.Current, data.Base.Scalings);
        target.ModifyStat(stat.Definition.Name, -modifier);
        if(data.SendTrigger) { BattleManager.current.EffectController.OnDebuff.TriggerEffect(source, target); }
        await Task.Delay(100);
        data.OnComplete();
    }

    public async void ModifyCooldown(Creature source, Creature target, DynamicEffectData data)
    {
        Spell spell = target.FindSpell(data.Spell);
        if(spell != null) { spell.ModifyStat("Cooldown", data.Stat.Current); }
        await Task.Delay(100);
        data.OnComplete();
    }

    public async void ModifyDuration(Creature source, Creature target, DynamicEffectData data)
    {
        Status status = target.FindStatus(data.Status);
        if(status != null) { status.ModifyStat("Duration", data.Stat.Current); }
        await Task.Delay(100);
        data.OnComplete();
    }

    public async void Afflict(Creature source, Creature target, DynamicEffectData data)
    {
        Status status = target.FindStatus(data.Status);
        int duration = source.ApplyScaling(data.Stat.Current, data.Base.Scalings);
        duration = Mathf.Clamp(duration - target.Resistance.Current, 1, duration);
        if(status == null)
        {
            status = Instantiate(data.Status, target.transform.position, Quaternion.identity);
            status.SetBase(target);
            status.SetStat("Duration", duration);
            target.AddStatus(status);
            if(data.SendTrigger) { BattleManager.current.EffectController.OnAfflict.TriggerEffect(source, target); }
        }
        else { status.ModifyStat("Duration", duration); }
        await Task.Delay(100);
        data.OnComplete();
    }

    public async void Cure(Creature source, Creature target, DynamicEffectData data)
    {
        Status status = target.FindStatus(data.Status);
        if(status != null)
        {
            target.RemoveStatus(status);
            if(data.SendTrigger) { BattleManager.current.EffectController.OnCure.TriggerEffect(source, target); }
        }
        await Task.Delay(100);
        data.OnComplete();
    }
}