using System;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

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

    public List<EffectData> Effects { get; private set; }
    public bool Pending { get; private set; }

    public Action OnEffectComplete { get; set; }

    EffectData data;

    public void SetBase()
    {
        Effects = new List<EffectData>();
        Pending = false;
    }

    public void Enqueue(EffectData data)
    {
        Effects.Add(data);
        if(!Pending) { Activate(); }
    }

    public void Activate()
    {
        if(Effects.Count <= 0) { return; }
        data = Effects[0];
        Pending = true;
        data.OnComplete = OnComplete;
        data.Base.Function.Invoke(data);
    }

    private async void OnComplete()
    {
        await Task.Delay(100);
        Pending = false;
        Effects.Remove(data);
        Debug.Log($"num effects: {Effects.Count}");
        if(Effects.Count <= 0) { OnEffectComplete(); }
        else { Activate(); }
    }
}