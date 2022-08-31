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

    // exploration effects

        public void ModifyStats(ChoiceData data)
    {
        foreach(StatBase statBase in data.Base.Stats)
        {
            Stat stat = data.Owner.FindStat(statBase.Definition);
            stat.Modify(statBase.Current);
            data.Owner.UI.UpdateUI();
        }
        data.OnComplete();
    }

    public void SetStats(ChoiceData data)
    {
        foreach(StatBase statBase in data.Base.Stats)
        {
            Stat stat = data.Owner.FindStat(statBase.Definition);
            stat.Set(statBase.Current);
        }
        data.OnComplete();
    }

    public void AddStatus(ChoiceData data)
    {
        Status temp = Instantiate(data.Base.Status);
        temp.SetBase(data.Owner);
        data.Owner.AddStatus(temp);
        data.OnComplete();
    }

    public void RemoveStatus(ChoiceData data)
    {
        if(data.Base.Status != null)
        {
            Status temp = data.Owner.FindStatus(data.Base.Status);
            if(temp != null) { data.Owner.RemoveStatus(temp); }
            data.OnComplete();
        }
        else
        {
            ExplorationManager.current.Selector.OnSelectStatus = (() =>
            {
                data.Owner.RemoveStatus(ExplorationManager.current.Selector.Status);
                data.OnComplete();
                ExplorationManager.current.Selector.Status = null;
                ExplorationManager.current.Selector.OnSelectStatus = null;
            });
        }
    }

    public void AddSpell(ChoiceData data)
    {
        Spell temp = Instantiate(data.Base.Spell);
        temp.SetBase(data.Owner);
        data.Owner.AddSpell(temp);
        data.OnComplete();
    }

    public void RemoveSpell(ChoiceData data)
    {
        if(data.Base.Spell != null)
        {
            Spell temp = data.Owner.FindSpell(data.Base.Spell);
            if(temp != null) { data.Owner.RemoveSpell(temp); }
            data.OnComplete();
        }
        else
        {
            ExplorationManager.current.Selector.OnSelectSpell = (() =>
            {
                data.Owner.RemoveSpell(ExplorationManager.current.Selector.Spell);
                data.OnComplete();
                ExplorationManager.current.Selector.Spell = null;
                ExplorationManager.current.Selector.OnSelectSpell = null;
            });
        }
    }

    public void AddItem(ChoiceData data)
    {

    }

    public void RemoveItem(ChoiceData data)
    {
        if(data.Base.Item != null)
        {
            Item temp = data.Owner.FindItem(data.Base.Item);
            if(temp != null) { data.Owner.RemoveItem(temp); }
            data.OnComplete();
        }
        else
        {
            ExplorationManager.current.Selector.OnSelectItem = (() =>
            {
                ExplorationManager.current.Player.RemoveItem(ExplorationManager.current.Selector.Item);
                data.OnComplete();
                ExplorationManager.current.Selector.Item = null;
                ExplorationManager.current.Selector.OnSelectItem = null;
            });
        }
    }

    public void AddEquipment(ChoiceData data)
    {

    }

    public void RemoveEquipment(ChoiceData data)
    {
        if(data.Base.Equipment != null)
        {
            Equipment temp = data.Owner.FindEquipment(data.Base.Equipment);
            if(temp != null) { data.Owner.RemoveEquipment(temp); }
            data.OnComplete();
        }
        else
        {
            ExplorationManager.current.Selector.OnSelectEquipment = (() =>
            {
                data.Owner.RemoveEquipment(ExplorationManager.current.Selector.Equipment);
                data.OnComplete();
                ExplorationManager.current.Selector.Equipment = null;
                ExplorationManager.current.Selector.OnSelectEquipment = null;
            });
        }
    }
}