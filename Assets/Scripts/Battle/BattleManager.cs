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
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(1))
        {
            SelectedSpell = null;
            TargetController.EnableSelection(false);
        }
    }

    public void EnterBattle(Creature player, BattleLayout layout)
    {
        UI.gameObject.SetActive(true);
        TurnController.gameObject.SetActive(true);
        TargetController.gameObject.SetActive(true);
        EffectController.gameObject.SetActive(true);

        UI.SetUI(layout);
        AddPlayer(player);
        foreach(CreatureContainer friend in UI.Friends) { AddFriend(friend.Default); }
        foreach(CreatureContainer enemy in UI.Enemies) { AddEnemy(enemy.Default); }

        TurnController.SetTurnOrder();
        TurnController.SortTurnOrder();

        TargetController.EnableSelection(false);
        
        StartCoroutine(StartTurn());
    }

    public void AddPlayer(Creature creature)
    {
        if(creature == null) { return; }
        CreatureContainer container = UI.FindEmptyPlayer();
        if(container == null) { return; }
        container.Add(creature);
        TurnController.Add(creature);
        TargetController.AddPlayer(creature);
    }

    public void AddFriend(Creature creature)
    {
        if(creature == null) { return; }
        CreatureContainer container = UI.FindEmptyFriend();
        if(container == null) { return; }
        Creature temp = Instantiate(creature, container.transform.position, Quaternion.identity);
        temp.SetBase();
        container.Add(temp);
        TurnController.Add(temp);
        TargetController.AddFriend(temp);
    }

    public void AddEnemy(Creature creature)
    {
        if(creature == null) { return; }
        CreatureContainer container = UI.FindEmptyEnemy();
        if(container == null) { return; }
        Creature temp = Instantiate(creature, container.transform.position, Quaternion.identity);
        temp.SetBase();
        container.Add(temp);
        TurnController.Add(temp);
        TargetController.AddEnemy(temp);
    }

    public void RemoveCreature(Creature creature)
    {
        CreatureContainer container = UI.FindContainer(creature);
        container.Remove();
        TurnController.Remove(creature);
        TargetController.Remove(creature);
        Destroy(creature.gameObject);
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
        ErrorCheck();
        if(TurnController.ActiveCreature == null) { StartCoroutine(StartTurn()); }
    }

    public IEnumerator StartTurn()
    {
        TurnController.FindActiveCreature();
        TurnController.ActiveCreature.UI.ShowActiveIndicator(true);
        EffectController.OnTurnStart.TriggerEffect(TurnController.ActiveCreature, TurnController.ActiveCreature);
        yield return new WaitUntil(() => EffectController.Effects.Count <= 0);
        ErrorCheck();
        if(TurnController.ActiveCreature == null) { StartCoroutine(StartTurn()); }
        else { StartCoroutine(MainPhase()); }
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
            if(TurnController.ActiveCreature == null) { StartCoroutine(StartTurn()); }
            else { EndTurn();  }
        }
        else
        {
            UI.EnableEndTurn(true);
            TurnController.ActiveCreature.EnableSpells(true);
        }
    }
    
    public void EndTurn()
    {
        TurnController.ActiveCreature.EnableSpells(false);
        TurnController.ActiveCreature.UI.ShowActiveIndicator(false);
        TargetController.EnableSelection(false);

        UI.EnableEndTurn(false);

        EffectController.OnTurnEnd.TriggerEffect(TurnController.ActiveCreature, TurnController.ActiveCreature);
        // yield return new WaitUntil(() => EffectController.Effects.Count <= 0);
        StartCoroutine(StartTurn());
    }

    public void ErrorCheck()
    {
        Creature player = UI.Player.Creature;
        if(player.Health.Current <= 0)
        {
            TurnController.Remove(player);
            TargetController.Remove(player);
            UI.Player.Remove();
            Destroy(player.gameObject);
        }
        foreach(CreatureContainer container in UI.Friends)
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
        foreach(CreatureContainer container in UI.Enemies)
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
    }

    public void ExitBattle()
    {
        UI.SetBase();
        UI.gameObject.SetActive(false);

        TurnController.SetBase();
        TurnController.gameObject.SetActive(false);

        TargetController.SetBase();
        TargetController.gameObject.SetActive(false);

        EffectController.SetBase();
        EffectController.gameObject.SetActive(false);
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