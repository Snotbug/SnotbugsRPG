using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
[System.Serializable]
public class StatBlock : MonoBehaviour
{

    public enum StatType
    {
        // this is the OFFICIAL ORDER
        HP = 0,
        MP = 1,
        STAM = 2,
        STR = 3,
        DEX = 4,
        KNOW = 5,
        SPD = 6,
        Max //Keep this Enum as the last Index
        
    }
    public enum StatArrays
        {
            Base,
            Level,
            Additional
        }

        public List<GameStat> this[StatArrays a]
        {
            //this will give us access to lists directly from the statblock, no need for a setter
            //since they are lists
            get
            {
                switch (a)
                {
                    
                    case StatArrays.Base:
                        return baseStats;
                        break;
                    case StatArrays.Level:
                        return leveledStats;
                        break;
                    case StatArrays.Additional:
                        return additionalStats;
                        break;
                    default:
                      //  return null; 
                        throw new ArgumentOutOfRangeException(nameof(a), a, null);
                }
            }
        }
        
        public int currentLevel
        {
            get
            {
                int level = leveledStats[0].value + leveledStats[1].value + leveledStats[2].value + additionalStats[3].value; 
                
                //print("Current Level:" + level);
                return level; 
            }
        }
        public int costOfNextLevel
        {
            get
            {
                int cost =  Mathf.RoundToInt(GeometricSeries(400f, 1.2f, currentLevel + 1));
                print("Cost: " + cost) ;
                return cost; 
            }
        }
        public static float GeometricSeries(float a, float r, int n)
        {
            return a * (Mathf.Pow(r, n - 1));
            //  return a * (1 - Mathf.Pow(r, n + 1)) / (1 - r);
        }

        [InfoBox("These stats are scaled different than expected")]
        public List<GameStat> baseStats = GameStat.CreateNewArray().ToList(); 
        
        [InfoBox("These stats pull values from the Inventory and will reset them when you level")]
        public List<GameStat> leveledStats  = GameStat.CreateNewArray().ToList();


        [InfoBox("values added manually must be removed manually")]
        public List<GameStat> additionalStats = new List<GameStat>();  

        // we could prolly make a system that puts these inside Extra Stats, but I wanted to seperate them so we know the are more important. 
        //and it was faster than writing a bunch of util
        public GameStat[] BespokeUpgrade = GameStat.CreateNewArray();

        
        [System.Serializable]
        public class GameStat
        {
            public string name; // Only for Editor Usability, needs to be turned on
            public StatType type;
            [OnValueChanged("RefreshName")]
            public int value; 
            public string source;
            public GameStat(StatType t, int v, string s)
            {
                type = t;
                value = v;
                source = s;
                RefreshName();
            }
            public void RefreshName()
            {
                name = type.ToString() + ": " + value; 
            }
            
            public static GameStat[] CreateNewArray()
            {
                GameStat[] gameStats = new GameStat[(int)StatType.Max];
                Array eArray = Enum.GetValues(typeof(StatType)); 
                
                for (int i = 0; i < (int)StatType.Max ; i++)
                {
                    gameStats[i] = new GameStat((StatType)eArray.GetValue(i),0,Enum.GetName(typeof(StatType),i)); 
                }
                return gameStats; 
            }
        }

        // We can create more dynamic lists if we wanted :/ dealers choice f


        public void Init()
        {
            InitBaseStats();
            SetLevelStats();
        }

        public void InitBaseStats()
        {
            //this array is fixed and will never change (...unless) 
            baseStats = GameStat.CreateNewArray().ToList(); 
        }
        

        [Button()]
        public void SetLevelStats() 
        {
        //    leveledStats = new List<GameStat>
          //  {
         //       new GameStat(StatType.CON, SaveFile.current.inventoryData["Level_CON"].amount, "Levels "),
         //       new GameStat(StatType.DEF, SaveFile.current.inventoryData["Level_DEF"].amount, "Levels "),
         //       new GameStat(StatType.ATK, SaveFile.current.inventoryData["Level_ATK"].amount, "Levels "),
         //       new GameStat(StatType.MAG, SaveFile.current.inventoryData["Level_MAG"].amount, "Levels ")
         //   }; 
        }

        //public float GetDamageResist()
        //
        //{
        //return GetResistFromDef(GetStatTotal(StatType.DEF));
        //}
        
    
        public static int defToHalf = 30; 
        //look I got my pemdas wrong and was gaslit(by myself) into just writing the steps out so I dont have to fick with pemdas 
        public static float GetDEFFromResist(float resist)
        {    
            float a = defToHalf/(1 -resist);
            float b = a - defToHalf;
            return b; 

        }
        public static float GetResistFromDef(int DEF)
        {    
            float a = (defToHalf + DEF);
            float b = defToHalf / a; 
            return  1 - b;

        }


  
        //
 
        [Button()]
        public void CalculateDamageResist()
        {
            // 
            float a = (defToHalf + GetDEFFromResist(.5f));
            float b = defToHalf / a; 
            print(  1 - b);
        }

        public float CalculateDamageMod(StatType dType,float scale) // lol with this you could calculate damage with the same number based on a diff stat but like, who cares 
        {
            return (GetStatTotal(dType) * scale) - 0.05f; 
        }
        public float CalculateTotalHealth(float healthScale) //scale being "How much Health value per HealthStat" 
        {
            return (GetStatBaseTotal(StatType.HP) * GetExtraStatTotal(StatType.HP))* healthScale; 
        }
        public float CalculateBaseHealth(float hpScale)
        {
            return GetStatTotal(StatType.HP) * hpScale; 
        }
        public float CalculateExtraHealth(float hpScale)
        {
            return GetExtraStatTotal(StatType.HP) * hpScale; 
        }

        
        [Button()]
        public void TestCalc()
        {
            print(GetStatTotal(StatType.HP));
        }
        
        
         //this gets the total number of a stat that is inside the base + the Leveled groups (FIT includes the upgrades)
        public int GetStatBaseTotal(StatType t)
        {
            var total = 0;
            if (BespokeUpgrade[(int)t] != null)
                total += BespokeUpgrade[(int)t].value;
            return Mathf.Clamp(total,0,999);
        }
        
        public int GetExtraStatTotal(StatType t)  // this returns the TOTAL NUMBER of a Stat that is inside the statblock. 
        {

            int total = 0;
            int count = additionalStats.Count; 
            for (var i = 0; i < count; i++)
            {
                GameStat stat = additionalStats[i]; //one access instead of repeat accesses 
                if (stat.type == t)
                    total += stat.value;
            }
            return Mathf.Clamp(total,0,999);
        }
        
      
        public int GetStatTotal(StatType t)  // this returns the TOTAL NUMBER of a Stat that is inside the statblock. 
        {

            int total = 0;
            total = Mathf.Clamp(GetStatBaseTotal(t) + GetExtraStatTotal(t),0,999);
            return total;
        }
        
       
        // Ensure you pass a ref to this list, not create a new stat that doesnt live anywhere else. 
        //if you do it will never get out
        public void AddNewStatModifier(GameStat newStat)
        {
         
            additionalStats.Add(newStat);
            TriggerStatBlockChanged();
            //EVENT THAT TELLS THE GAME TO CHECK FOR CHANGES???
        }
        public void RemoveStatModifier(GameStat oldStat) // called from an object elsewhere
        {
            if (oldStat is null)
            {
                Debug.LogError("Attempted to Remove a Stat that was already Dead");
                CullNullStats();
                return;
            }
            if (additionalStats.Contains(oldStat)) { additionalStats.Remove(oldStat); }
            TriggerStatBlockChanged();
        }
        
        public void SetBaseStat(StatType statType, int value)
        {
            baseStats[(int)statType].value = value; 
        }
       
        public void CullNullStats() //This is a backup and shouldnt get used (hopefully) 
        {
            for (var i = additionalStats.Count - 1; i > -1; i--)
                if (additionalStats[i] == null)
                    additionalStats.RemoveAt(i);
        }
        
        
        // Move this to the game Manager Later. 
        public delegate void StatBlockChanged(); 

        
        public event StatBlockChanged OnStatBlockChanged;

        
        public void TriggerStatBlockChanged()
        {
            if (OnStatBlockChanged != null)
            {
                try
                {
                    OnStatBlockChanged();
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                }
            }
        }

        // gives you the number of stats it would take to give you a desired flat multiplier
        // (gives you enough of a stat to achieve the goal multiplier) 
        public static int GetFlatStatFromMulti(float valueScale, float value)
        {
            // do we want to round or
            return Mathf.RoundToInt(value / valueScale); 
        }
        
        
        // Gives you the number of stats as a result of a multiplier from Base + Leveld Stats (if you have 50 atk and pass .5, youll get 25)
        public int MultiplyBaseStat(StatType t, float value)
        {
            return Mathf.RoundToInt(GetStatBaseTotal(t) * value); 
        }

 
        
        
      
    
}
