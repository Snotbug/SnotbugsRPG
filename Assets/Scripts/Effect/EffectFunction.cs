using UnityEngine;

[CreateAssetMenu(fileName = "EffectFunction", menuName = "EffectFunction")]
public class EffectFunction : ScriptableObject
{
    public void SpawnFriend(EffectData data)
    {
        TargetController targetController = BattleManager.current.TargetController;
        if(targetController.IsEnemy(data.Source)) { BattleManager.current.AddEnemy(data.Creature); }
        else { BattleManager.current.AddFriend(data.Creature); }
        data.OnComplete();
    }

    public void SpawnEnemy(EffectData data)
    {
        TargetController targetController = BattleManager.current.TargetController;
        if(targetController.IsEnemy(data.Source)) { BattleManager.current.AddFriend(data.Creature); }
        else { BattleManager.current.AddEnemy(data.Creature); }
    }

    public void Damage(EffectData data)
    {
        int damage = data.Source.ApplyScaling(data.Stat.Current, data.Base.Scalings);
        damage = Mathf.Clamp(damage - data.Target.Resistance.Current, 1, data.Target.Health.Max);
        bool isDead = data.Target.Health.Current <= 0;
        data.Target.ModifyStat("Health", -damage);
        if(data.SendTrigger) { BattleManager.current.EffectController.OnDamage.TriggerEffect(data.Source, data.Target); Debug.Log($"{data.Source.Base.Name} damaged {data.Target.Base.Name} for {-damage}"); }
        if(!isDead && data.Target.Health.Current <= 0) { BattleManager.current.EffectController.OnDeath.TriggerEffect(data.Source, data.Target); Debug.Log($"{data.Source.Base.Name} killed {data.Target.Base.Name}"); }
        data.OnComplete();
    }

    public void Heal(EffectData data)
    {
        int heal = data.Source.ApplyScaling(data.Stat.Current, data.Base.Scalings);
        data.Target.ModifyStat("Health", heal);
        if(data.SendTrigger) { BattleManager.current.EffectController.OnHeal.TriggerEffect(data.Source, data.Target); }
        data.OnComplete();
    }

    public void Buff(EffectData data)
    {
        Stat stat = data.Target.FindStat(data.Stat.Definition);
        int modifier = data.Source.ApplyScaling(data.Stat.Current, data.Base.Scalings);
        data.Target.ModifyStat(stat.Definition.Name, modifier);
        if(data.SendTrigger) { BattleManager.current.EffectController.OnBuff.TriggerEffect(data.Source, data.Target); }
        data.OnComplete();
    }

    public void DeBuff(EffectData data)
    {
        Stat stat = data.Target.FindStat(data.Stat.Definition);
        int modifier = data.Source.ApplyScaling(data.Stat.Current, data.Base.Scalings);
        data.Target.ModifyStat(stat.Definition.Name, -modifier);
        if(data.SendTrigger) { BattleManager.current.EffectController.OnDebuff.TriggerEffect(data.Source, data.Target); }
        data.OnComplete();
    }

    public void Equip(EffectData data)
    {
        Stat stat = data.Target.FindStat(data.Stat.Definition);
        int modifier = data.Source.ApplyScaling(data.Stat.Current, data.Base.Scalings);
        data.Target.ModifyStat(stat.Definition.Name, -modifier);
        if(data.SendTrigger) { BattleManager.current.EffectController.OnDebuff.TriggerEffect(data.Source, data.Target); }
        data.OnComplete();
    }

    public void ModifyCooldown(EffectData data)
    {
        Spell spell = data.Target.FindSpell(data.Spell);
        if(spell != null) { spell.ModifyStat("Cooldown", data.Stat.Current); }
        Debug.Log($"{spell.Base.Name}: {spell.Cooldown.Current}");
        data.OnComplete();
    }

    public void ModifyDuration(EffectData data)
    {
        Status status = data.Target.FindStatus(data.Status);
        if(status != null) { status.ModifyStat("Duration", data.Stat.Current); }
        data.OnComplete();
    }

    public void Afflict(EffectData data)
    {
        Debug.Log("afflicting");
        Status status = data.Target.FindStatus(data.Status);
        int duration = data.Source.ApplyScaling(data.Stat.Current, data.Base.Scalings);
        duration = Mathf.Clamp(duration - data.Target.Resistance.Current, 1, duration);
        if(status == null)
        {
            status = Instantiate(data.Status, data.Target.transform.position, Quaternion.identity);
            status.SetBase(data.Target);
            status.SetStat("Duration", duration);
            data.Target.AddStatus(status);
            if(data.SendTrigger) { BattleManager.current.EffectController.OnAfflict.TriggerEffect(data.Source, data.Target); }
        }
        else { status.ModifyStat("Duration", duration); }
        data.OnComplete();
    }

    public void Cure(EffectData data)
    {
        Status status = data.Target.FindStatus(data.Status);
        if(status != null)
        {
            data.Target.RemoveStatus(status);
            if(data.SendTrigger) { BattleManager.current.EffectController.OnCure.TriggerEffect(data.Source, data.Target); }
        }
        data.OnComplete();
    }

        // public void Equip(Equipment equipment)
    // {
    //     foreach(StatBase requirement in equipment.Base.Requirement)
    //     {
    //         Stat stat = FindStat(requirement.Definition);
    //         if(stat.Current < requirement.Current || stat.Max < requirement.Max) { return; }
    //     }

    //     foreach(StatBase modifier in equipment.Base.Modifiers)
    //     {
    //         Stat stat = FindStat(modifier.Definition);
    //         stat.Modify(modifier.Current);
    //         stat.ModifyMax(modifier.Max);
    //     }
    // }

    // public void Unequip(Equipment equipment)
    // {
    //     foreach(StatBase modifier in equipment.Base.Modifiers)
    //     {
    //         Stat stat = FindStat(modifier.Definition);
    //         stat.Modify(-modifier.Current);
    //         stat.ModifyMax(-modifier.Max);
    //     }
    // }
}