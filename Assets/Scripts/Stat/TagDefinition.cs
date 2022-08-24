using UnityEngine;

[CreateAssetMenu(fileName = "Tag", menuName = "Tag")]
public class TagDefinition : ScriptableObject
{
    [field : SerializeField] public string Name { get; private set; }
}
