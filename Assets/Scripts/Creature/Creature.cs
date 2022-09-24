using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Creature : MonoBehaviour
{
    [field : SerializeField] public CreatureBase Base { get; private set; }
    [field : SerializeField] public CreatureUI UI { get; private set; }
    [field : SerializeField] public CreatureAnimator Animator { get; private set; }

    public Stat Health { get { return Stats["Health"]; }}
    public Stat Mana { get { return Stats["Mana"]; }}
    public Stat Stamina { get { return Stats["Stamina"]; }}

    public Stat Strength { get { return Stats["Strength"]; }}
    public Stat Dexterity { get { return Stats["Dexterity"]; }}
    public Stat Knowledge { get { return Stats["Knowledge"]; }}
    public Stat Resistance { get { return Stats["Resistance"]; }}
    public Stat Speed { get { return Stats["Speed"]; }}

    public Dictionary<string, Stat> Stats { get; private set; }
    public List<Status> Statuses { get; private set; }
    public List<Spell> Spells { get; private set; }

    public void SetBase()
    {
        Stats = new Dictionary<string, Stat>();
        foreach(StatBase stat in Base.Stats)
        {
            Stat temp = new Stat(stat.Definition, stat.Current, stat.Max);
            Stats.Add(stat.Definition.Name, temp);
        };

        Statuses = new List<Status>();
        foreach(Status status in Base.Statuses)
        {
            Status temp = Instantiate(status, this.transform.position, Quaternion.identity);
            temp.transform.SetParent(this.transform);
            temp.SetBase(this);
            AddStatus(temp);
        }

        Spells = new List<Spell>();
        foreach(Spell spell in Base.Spells)
        {
            Spell temp = Instantiate(spell, this.transform.position, Quaternion.identity);
            temp.transform.SetParent(this.transform);
            temp.SetBase(this);
            AddSpell(temp);
        }

        UI.SetUI(this);
    }

    public void OnDestroy()
    {
        if(UI != null) { Destroy(UI.gameObject); }
        if(Animator != null) { Destroy(Animator?.gameObject); }
    }

    public void ResetStats()
    {
        foreach(KeyValuePair<string, Stat> pair in Stats)
        {
            Stat stat = pair.Value;
            if(!stat.Definition.IsResource)
            {
                stat.Set(stat.Max);
                UI.UpdateUI();
            }
        }
    }

    public Stat FindStat(string name)
    {
        return Stats.ContainsKey(name) ? Stats[name] : null;
    }

    public Stat FindStat(StatDefinition definition)
    {
        return Stats.ContainsKey(definition.Name) ? Stats[definition.Name] : null;
    }

    public int ApplyScaling(int value, List<StatBase> scalings)
    {
        foreach(StatBase scaling in scalings)
        {
            Stat stat = FindStat(scaling.Definition);
            value += (int)((stat.Current * (float)scaling.Current / 100) + (stat.Max * (float)scaling.Max / 100));
        }
        return value;
    }

    public void ModifyStat(string name, int amount)
    {
        Stats[name].Modify(amount);
        UI.UpdateUI();
    }

    public void SetStat(string name, int value)
    {
        Stats[name].Set(value);
        UI.UpdateUI();
    }

    public void ModifyMaxStat(string name, int amount)
    {
        Stats[name].ModifyMax(amount);
    }

    public void SetMaxStat(string name, int value)
    {
        Stats[name].SetMax(value);
    }

    public Status FindStatus(Status status)
    {
        return (Status)Statuses.FirstOrDefault(n => n.Base.Name.Equals(status.Base.Name));
    }

    public void AddStatus(Status status)
    {
        if(FindStatus(status) != null) { return; }
        Statuses.Add(status);
        status.transform.SetParent(this.transform);

        if(UI != null) { UI.AddStatus(status); }
    }

    public void RemoveStatus(Status status)
    {
        Status foundStatus = FindStatus(status);
        if(foundStatus == null) { return; }
        Destroy(foundStatus.gameObject);
        Statuses.Remove(foundStatus);
    }

    public Spell FindSpell(Spell spell)
    {
        return (Spell)Spells.FirstOrDefault(n => n.Base.Name.Equals(spell.Base.Name));
    }

    public bool CanActivate(Spell spell)
    {
        if
        (
            spell.Cooldown.Current > 0
        )
        { return false; }
        foreach(StatBase cost in spell.Base.Costs)
        {
            if(FindStat(cost.Definition).Current < cost.Current) { return false; }
        }
        return true;
    }

    public void PayCost(List<StatBase> costs)
    {
        foreach(StatBase cost in costs) { ModifyStat(cost.Definition.Name, -cost.Current); }
        UI.UpdateUI();
    }

    public void EnableSpells(bool enable)
    {
        foreach(Spell spell in Spells)
        {
            spell.UI.gameObject.SetActive(enable);
        }
        // if(enable) { foreach(Spell spell in Spells) { spell.UI.SetInteractable(CanActivate(spell)); }}
        // else{ foreach(Spell spell in Spells) { spell.UI.SetInteractable(false); }}
    }

    public List<Spell> FindActivatable()
    {
        List<Spell> spells = new List<Spell>();
        foreach(Spell spell in Spells) { if(CanActivate(spell)) { spells.Add(spell); }}
        return spells;
    }

    public void AddSpell(Spell spell)
    {
        if(FindSpell(spell) != null) { return; }
        Spells.Add(spell);
        spell.transform.SetParent(this.transform);

        if(UI != null) { UI.AddSpell(spell); }
    }

    public void RemoveSpell(Spell spell)
    {
        Spell foundSpell = FindSpell(spell);
        if(foundSpell == null) { return; }
        Destroy(foundSpell.gameObject);
        Spells.Remove(foundSpell);
    }
}