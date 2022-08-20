using UnityEngine;

[CreateAssetMenu(fileName = "EffectFunction", menuName = "EffectFunction")]
public class EffectFunction : ScriptableObject
{
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

    public void Damage(Creature source, Creature target, DynamicEffectData data)
    {
        int damage = source.ApplyScaling(data.Stat.Current, data.Base.Scalings);
        damage = Mathf.Clamp(damage - target.Resistance.Current, 1, target.Health.Max);
        bool isDead = target.Health.Current <= 0;
        target.ModifyStat("Health", -damage);
        if(data.SendTrigger) { BattleManager.current.EffectController.OnDamage.TriggerEffect(source, target); Debug.Log($"{source.Base.Name} damaged {target.Base.Name} for {-damage}"); }
        if(!isDead && target.Health.Current <= 0) { BattleManager.current.EffectController.OnDeath.TriggerEffect(source, target); Debug.Log($"{source.Base.Name} killed {target.Base.Name}"); }
        data.OnComplete();
    }

    public void Heal(Creature source, Creature target, DynamicEffectData data)
    {
        int heal = source.ApplyScaling(data.Stat.Current, data.Base.Scalings);
        target.ModifyStat("Health", heal);
        if(data.SendTrigger) { BattleManager.current.EffectController.OnHeal.TriggerEffect(source, target); }
        data.OnComplete();
    }

    public void Buff(Creature source, Creature target, DynamicEffectData data)
    {
        Stat stat = target.FindStat(data.Stat.Definition);
        int modifier = source.ApplyScaling(data.Stat.Current, data.Base.Scalings);
        target.ModifyStat(stat.Definition.Name, modifier);
        if(data.SendTrigger) { BattleManager.current.EffectController.OnBuff.TriggerEffect(source, target); }
        data.OnComplete();
    }

    public void DeBuff(Creature source, Creature target, DynamicEffectData data)
    {
        Stat stat = target.FindStat(data.Stat.Definition);
        int modifier = source.ApplyScaling(data.Stat.Current, data.Base.Scalings);
        target.ModifyStat(stat.Definition.Name, -modifier);
        if(data.SendTrigger) { BattleManager.current.EffectController.OnDebuff.TriggerEffect(source, target); }
        data.OnComplete();
    }

    public void ModifyCooldown(Creature source, Creature target, DynamicEffectData data)
    {
        Spell spell = target.FindSpell(data.Spell);
        if(spell != null) { spell.ModifyStat("Cooldown", data.Stat.Current); }
        data.OnComplete();
    }

    public void ModifyDuration(Creature source, Creature target, DynamicEffectData data)
    {
        Status status = target.FindStatus(data.Status);
        if(status != null) { status.ModifyStat("Duration", data.Stat.Current); }
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
            status.SetStat("Duration", duration);
            target.AddStatus(status);
            if(data.SendTrigger) { BattleManager.current.EffectController.OnAfflict.TriggerEffect(source, target); }
        }
        else { status.ModifyStat("Duration", duration); }
        data.OnComplete();
    }

    public void Cure(Creature source, Creature target, DynamicEffectData data)
    {
        Status status = target.FindStatus(data.Status);
        if(status != null)
        {
            target.RemoveStatus(status);
            if(data.SendTrigger) { BattleManager.current.EffectController.OnCure.TriggerEffect(source, target); }
        }
        data.OnComplete();
    }
}