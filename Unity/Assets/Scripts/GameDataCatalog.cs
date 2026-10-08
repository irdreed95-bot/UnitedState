using System;using System.Collections.Generic;using UnityEngine;
namespace CorruptStateRP {
public static class GameDataCatalog {
 [Serializable] public class Job { public string id; public string name; public int salary; }
 [Serializable] public class JobsData { public Job[] jobs; }
 [Serializable] public class Faction { public string name; public int rankCount; }
 [Serializable] public class FactionsData { public Faction police,military,medical,civil_defense,government,justice,prisons,bank,national_security; }
 [Serializable] public class StreetsData { public string[] streets; }
 public static JobsData Jobs {get;private set;} public static FactionsData Factions {get;private set;} public static StreetsData Streets {get;private set;}
 public static void LoadAll(){Jobs=Load<JobsData>("Data/jobs");Factions=Load<FactionsData>("Data/factions");Streets=Load<StreetsData>("Data/streets");if(Jobs?.jobs==null||Factions==null||Streets?.streets==null)throw new InvalidOperationException("Required Assets/Resources/Data JSON could not be loaded.");if(Factions.medical.rankCount!=14||Streets.streets.Length!=22)throw new InvalidOperationException("Faction/street data integrity check failed.");Debug.Log($"Loaded data: {Jobs.jobs.Length} jobs, 9 factions, {Streets.streets.Length} streets.");}
 static T Load<T>(string path){var asset=Resources.Load<TextAsset>(path);if(asset==null)throw new InvalidOperationException("Missing Resources/"+path+".json");return JsonUtility.FromJson<T>(asset.text);}
}}