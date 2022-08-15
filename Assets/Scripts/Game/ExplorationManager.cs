// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class ExplorationManager : MonoBehaviour
// {
//     [SerializeField] List<EncounterBase> encounterBases;

//     [SerializeField] GameObject encounterChoiceSpawnPoint;
//     [SerializeField] EncounterChoiceUI choiceUIPrefab;

//     private List<Encounter> encounters;
//     private Encounter activeEncounter;

//     private List<EncounterChoiceUI> choiceUIs;

//     private Creature player;

//     public void OnEnable()
//     {
//         EventManager.current.onClickEncounterChoice += EnactChoiceConsequences;
//     }

//     public void OnDisable()
//     {
//         EventManager.current.onClickEncounterChoice -= EnactChoiceConsequences;
//     }
    
//     public void ManualStart(Creature player)
//     {
//         choiceUIs = new List<EncounterChoiceUI>();
//         EnterEncounter(player);
//     }

//     public void ManualUpdate()
//     {
//     }

//     public void EnterEncounter(Creature player)
//     {
//         this.player = player;

//         encounters = new List<Encounter>();
//         foreach(EncounterBase encounterBase in encounterBases)
//         {
//             encounters.Add(new Encounter(encounterBase));
//         }

//         GetRandomEncounter();
//         SetChoices();
//     }

//     public void GetRandomEncounter()
//     {
//         activeEncounter = encounters[Random.Range(0, encounters.Count)];
//     }

//     public void SetChoices()
//     {
//         List<EncounterChoice> choices = activeEncounter.Base.Choices;

//         foreach(EncounterChoice choice in choices)
//         {
//             EncounterChoiceUI choiceUI = Instantiate(choiceUIPrefab, encounterChoiceSpawnPoint.transform.position, Quaternion.identity);
//             choiceUI.transform.SetParent(encounterChoiceSpawnPoint.transform);
//             choiceUI.transform.localScale = encounterChoiceSpawnPoint.transform.localScale;
//             choiceUI.SetEncounterChoice(choice);

//             choiceUIs.Add(choiceUI);

//             foreach(Resources prerequisite in choice.Prerequisite.Resources)
//             {
//                 if(player.Resources[prerequisite.resource] < prerequisite.value)
//                 {
//                     choiceUI.EnableSelection(false);
//                 }
//             }
//             foreach(Stats prerequisite in choice.Prerequisite.Stats)
//             {
//                 if(player.StatsMax[prerequisite.stat] < prerequisite.value)
//                 {
//                     choiceUI.EnableSelection(false);
//                 }
//             }
//         }
//     }

//     public void EnactChoiceConsequences(EncounterChoice choice)
//     {
//         foreach(Resources consequence in choice.Consequence.Resources)
//         {
//             player.ChangeResource(consequence.resource, consequence.value);
//         }

//         foreach(Resources consequence in choice.Consequence.ResourcesMax)
//         {
//             player.ChangeResourceMax(consequence.resource, consequence.value);
//         }

//         foreach(Stats consequence in choice.Consequence.Stats)
//         {
//             player.ChangeStat(consequence.stat, consequence.value);
//         }

//         foreach(Stats consequence in choice.Consequence.StatsMax)
//         {
//             player.ChangeStatMax(consequence.stat, consequence.value);
//         }

//         foreach(CreatureBase enemyBase in choice.Consequence.Enemies)
//         {
//             ExitExploration();
//             EventManager.current.EnterBattle(player, enemyBase);
//             EventManager.current.ExitExploration();
//         }
//     }

//     public void ExitExploration()
//     {
//         foreach(EncounterChoiceUI choiceUI in choiceUIs)
//         {
//             Destroy(choiceUI.gameObject);
//         }
//     }
// }
