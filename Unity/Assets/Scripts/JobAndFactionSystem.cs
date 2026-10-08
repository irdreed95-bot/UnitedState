using System.Collections.Generic;using UnityEngine;
namespace CorruptStateRP {
public sealed class JobAndFactionSystem:MonoBehaviour {
 public string CurrentJob="عاطل",CurrentFaction="مدني";public int Salary;public IReadOnlyDictionary<string,int> Salaries=>salaries;
 readonly Dictionary<string,int> salaries=new Dictionary<string,int>();
 void Awake(){if(GameDataCatalog.Jobs?.jobs!=null)foreach(var j in GameDataCatalog.Jobs.jobs)salaries[j.name]=j.salary;}
 public bool ChooseJob(string job){if(!salaries.TryGetValue(job,out var amount))return false;Salary=amount;CurrentJob=job;return true;}
 public bool JoinFaction(string faction){if(string.IsNullOrWhiteSpace(faction))return false;CurrentFaction=faction;return true;}
 public int CollectSalary(){Debug.LogWarning("Salary is server-authoritative; call /jobs/salary/claim instead of changing local money.");return 0;}
}}