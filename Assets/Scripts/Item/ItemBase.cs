using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Item")]
public class ItemBase : ScriptableObject
{
    [field : SerializeField] public string Name { get; private set; }
    [field : SerializeField] public string Description { get; private set; }

    [field : SerializeField] public List<StatBase> Stats { get; private set; }

    [field : SerializeField] public TargetType TargetType { get; private set; }
    [field : SerializeField] public EffectBase ActivatedEffect { get; private set; }
}