using System.Collections.Generic;using UnityEngine;using CorruptStateRP.Auth;
namespace CorruptStateRP {
public sealed class JobAndFactionSystem:MonoBehaviour {
 public string CurrentJob="عاطل",CurrentFaction="مدني";public int Salary;public IReadOnlyDictionary<string,int> Salaries=>salaries;
 readonly Dictionary<string,int> salaries=new Dictionary<string,int>();
 void Awake(){if(GameDataCatalog.Jobs?.jobs!=null)foreach(var j in GameDataCatalog.Jobs.jobs)salaries[j.name]=j.salary;}
 public bool ChooseJob(string job){if(!salaries.TryGetValue(job,out var amount))return false;Salary=amount;CurrentJob=job;return true;}
 public bool JoinFaction(string faction){if(string.IsNullOrWhiteSpace(faction))return false;CurrentFaction=faction;return true;}
 public int CollectSalary(){var auth=FindFirstObjectByType<AuthClient>();if(auth==null||!auth.HasSession){Debug.LogWarning("Salary claim requires a server session.");return 0;}StartCoroutine(auth.ClaimSalary((json,error)=>{if(error!=null)Debug.LogWarning("Salary claim rejected: "+error);else Debug.Log("Server salary claim response received.");}));return 0;}
}}