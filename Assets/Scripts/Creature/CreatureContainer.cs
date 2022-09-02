using UnityEngine;

public class CreatureContainer : MonoBehaviour
{
    [field : SerializeField] public Creature Default { get; private set; }
    
    public Creature Creature { get; private set; }

    public void Add(Creature creature)
    {
        if(!IsEmpty()) { return; }
        Creature = creature;
        Creature.transform.position = this.transform.position;
    }

    public void Remove()
    {
        if(IsEmpty()) { return; }
        Creature = null;
    }

    public bool IsEmpty() { return Creature == null; }
}
