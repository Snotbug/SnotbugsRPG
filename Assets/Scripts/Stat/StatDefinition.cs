using UnityEngine;

[CreateAssetMenu(fileName = "Stat", menuName = "Stat")]
public class StatDefinition : ScriptableObject
{
    [field : SerializeField] public string Name { get; private set; }
    [field : SerializeField] public bool IsResource { get; private set; }
}

[System.Serializable]
public class StatBase
{
    [field : SerializeField] public StatDefinition Definition { get; private set; }
    [field : SerializeField] public int Current { get; private set; }
    [field : SerializeField] public int Max { get; private set; }
}