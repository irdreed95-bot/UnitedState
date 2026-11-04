using System.Collections.Generic;
using UnityEngine;
namespace CorruptStateRP {
public sealed class JobAndFactionSystem:MonoBehaviour {
 public string CurrentJob="عاطل",CurrentFaction="مدني";
 public int Salary;
 public IReadOnlyDictionary<string,int> Salaries=>salaries;
 public IReadOnlyList<string> FactionNames=>factionNames;
 readonly Dictionary<string,int> salaries=new Dictionary<string,int>();
 readonly List<string> factionNames=new List<string>();
 void Awake(){
  if(GameDataCatalog.Jobs?.jobs!=null)foreach(var j in GameDataCatalog.Jobs.jobs)if(!string.IsNullOrWhiteSpace(j.name)&&j.salary>=0)salaries[j.name]=j.salary;
  var f=GameDataCatalog.Factions;
  if(f!=null){AddFaction(f.police);AddFaction(f.military);AddFaction(f.medical);AddFaction(f.civil_defense);AddFaction(f.government);AddFaction(f.justice);AddFaction(f.prisons);AddFaction(f.bank);AddFaction(f.national_security);}
  CurrentJob=PlayerPrefs.GetString("CSRP_CURRENT_JOB","عاطل");
  CurrentFaction=PlayerPrefs.GetString("CSRP_CURRENT_FACTION","مدني");
  if(CurrentJob!="عاطل"&&!salaries.ContainsKey(CurrentJob))CurrentJob="عاطل";
  if(CurrentFaction!="مدني"&&!factionNames.Contains(CurrentFaction))CurrentFaction="مدني";
  Salary=salaries.TryGetValue(CurrentJob,out var amount)?amount:0;
 }
 void AddFaction(GameDataCatalog.Faction faction){if(faction!=null&&!string.IsNullOrWhiteSpace(faction.name)&&!factionNames.Contains(faction.name))factionNames.Add(faction.name);}
 public bool ChooseJob(string job){if(!salaries.TryGetValue(job,out var amount))return false;Salary=amount;CurrentJob=job;PlayerPrefs.SetString("CSRP_CURRENT_JOB",job);PlayerPrefs.Save();return true;}
 public bool JoinFaction(string faction){if(!factionNames.Contains(faction))return false;CurrentFaction=faction;PlayerPrefs.SetString("CSRP_CURRENT_FACTION",faction);PlayerPrefs.Save();return true;}
 public int CollectSalary(){if(Salary<=0)return 0;return Salary;}
}}