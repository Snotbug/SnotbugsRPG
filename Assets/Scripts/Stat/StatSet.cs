using System.Collections.Generic;

public class StatSet
{
    public Dictionary<string, Stat> Stats { get; private set; }

    public StatSet(List<StatBase> stats)
    {
        Stats = new Dictionary<string, Stat>();
        foreach(StatBase stat in stats)
        {
            Stat temp = new Stat(stat.Definition, stat.Current, stat.Max);
            Stats.Add(stat.Definition.Name, temp);
        };
    }

    public Stat Find(string name)
    {
        return Stats.ContainsKey(name) ? Stats[name] : null;
    }

    public Stat Find(StatDefinition definition)
    {
        return Stats.ContainsKey(definition.Name) ? Stats[definition.Name] : null;
    }

    public void ResetStats()
    {
        foreach(KeyValuePair<string, Stat> pair in Stats)
        {
            Stat stat = pair.Value;
            if(!stat.Definition.IsResource)
            {
                stat.Set(stat.Max);
            }
        }
    }

    public int ApplyScaling(int value, List<StatBase> scalings)
    {
        foreach(StatBase scaling in scalings)
        {
            Stat stat = Find(scaling.Definition);
            value += (int)((stat.Current * (float)scaling.Current / 100) + (stat.Max * (float)scaling.Max / 100));
        }
        return value;
    }
}
