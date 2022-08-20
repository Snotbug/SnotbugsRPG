using System;
using System.Collections;
using UnityEngine;

public class Choice : MonoBehaviour
{
    [field : SerializeField] public ChoiceUI UI { get; private set; }

    public ChoiceBase Base { get; private set; }
    public bool Pending { get; private set; }

    public void SetBase(ChoiceBase choiceBase)
    {
        Base = choiceBase;
        Pending = false;

        UI.SetUI(this);
    }

    public bool CheckRequirements(Creature creature)
    {
        if
        (
            !CheckStats(creature) ||
            !CheckStatuses(creature) ||
            !CheckSpells(creature) ||
            !CheckItems(creature) ||
            !CheckEquipments(creature)
        ) { return false; }
        return true;
    }

    public bool CheckStats(Creature creature)
    {
        foreach(StatBase requirement in Base.Requirements.Stats)
        {
            Stat stat = creature.FindStat(requirement.Definition);
            if(stat.Current < requirement.Current || stat.Max < requirement.Max) { return false; }
        }
        return true;
    }

    public bool CheckStatuses(Creature creature)
    {
        Status requirement = Base.Requirements.Status;
        Status status = creature.FindStatus(requirement);
        if(status == null) { return false; }
        return true;
    }

    public bool CheckSpells(Creature creature)
    {
        Spell requirement = Base.Requirements.Spell;
        Spell spell = creature.FindSpell(requirement);
        if(spell == null) { return false; }
        return true;
    }

    public bool CheckItems(Creature creature)
    {
        Item requirement = Base.Requirements.Item;
        Item item = creature.FindItem(requirement);
        if(item == null) { return false; }
        return true;
    }

    public bool CheckEquipments(Creature creature)
    {
        Equipment requirement = Base.Requirements.Equipment;
        Equipment equipment = creature.FindEquipment(requirement);
        if(equipment == null) { return false; }
        return true;
    }

    public IEnumerator EnactConsequences()
    {
        ChoiceData data = new ChoiceData();
        foreach(ChoiceConsequence consequence in Base.Consequences)
        {
            Pending = true;
            data.SetBase(consequence.Data);
            data.OnComplete += OnComplete;
            consequence.Function.Invoke(data);
            yield return new WaitUntil(() => !Pending);
        }
    }

    public void OnComplete()
    {
        Pending = false;
    }

    public void OnDestroy()
    {
        if(UI != null) { Destroy(UI.gameObject); }
    }
}

public class ChoiceData
{
    public BaseChoiceData Base { get; private set; }
    public Action OnComplete { get; set; }

    public ChoiceData()
    {
        
    }

    public void SetBase(BaseChoiceData baseData)
    {
        Base = baseData;
    }
}