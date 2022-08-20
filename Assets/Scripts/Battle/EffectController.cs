using System;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EffectController : MonoBehaviour
{
    [field : SerializeField] public EffectTrigger OnTurnStart { get; private set; }
    [field : SerializeField] public EffectTrigger OnTurnEnd { get; private set; }
    [field : SerializeField] public EffectTrigger OnCast { get; private set; }
    [field : SerializeField] public EffectTrigger OnDamage { get; private set; }
    [field : SerializeField] public EffectTrigger OnHeal { get; private set; }
    [field : SerializeField] public EffectTrigger OnBuff { get; private set; }
    [field : SerializeField] public EffectTrigger OnDebuff { get; private set; }
    [field : SerializeField] public EffectTrigger OnAfflict { get; private set; }
    [field : SerializeField] public EffectTrigger OnCure { get; private set; }
    [field : SerializeField] public EffectTrigger OnDeath { get; private set; }

    public List<DynamicEffectData> Effects { get; private set; }
    public bool Pending { get; private set; }

    DynamicEffectData data;

    public void SetBase()
    {
        Effects = new List<DynamicEffectData>();
        Pending = false;
    }

    public void Enqueue(DynamicEffectData data)
    {
        Effects.Add(data);
        if(!Pending) { Activate(); }
    }

    public void Activate()
    {
        if(Effects.Count <= 0) { return; }
        data = Effects[0];
        Pending = true;
        data.OnComplete += OnComplete;
        data.Base.Function.Invoke(data.Owner, data.Target, data);
    }

    private async void OnComplete()
    {
        await Task.Delay(100);
        Pending = false;
        Effects.Remove(data);
        Activate();
    }
}