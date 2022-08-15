using UnityEngine;

public class CreatureSpawner : MonoBehaviour
{
    [field : SerializeField] public Creature CreaturePrefab { get; private set; }
    
    public Creature Creature { get; private set; }

    public void Spawn(Creature creature)
    {
        if(!IsEmpty()) { return; }

        Creature = Instantiate(creature, this.transform.position, Quaternion.identity);
        Creature.transform.SetParent(this.transform);
        Creature.SetBase();
    }

    public void Despawn()
    {
        if(IsEmpty()) { return; }

        Destroy(Creature.gameObject);
        Creature = null;
    }

    public void Set(Creature creature)
    {
        if(!IsEmpty() || creature == null) { return; }

        Creature = creature;
        Creature.transform.SetParent(this.transform);
    }

    public bool IsEmpty() { return Creature == null; }
}
