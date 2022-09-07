using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [field : SerializeField] public ItemBase Base { get; private set; }
    [field : SerializeField] public ItemUI UI { get; private set; }

    public Creature Owner { get; private set; }
    
    
    public Effect ActivatedEffect { get; private set; }

    public void SetBase(Creature owner)
    {
        Owner = owner;
        
        ActivatedEffect = new Effect(Base.ActivatedEffect, Owner);

        UI.SetUI(this);
    }

    public void ActivateQueued()
    {
 
        ActivatedEffect.QueueEffect(true);
    }
    

    public void OnDestroy()
    {
        if(UI != null) { Destroy(UI.gameObject); }
    }
}