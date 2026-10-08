using System.Collections.Generic;
using UnityEngine;

namespace CorruptStateRP {
public sealed class JobAndFactionSystem:MonoBehaviour{
 public string CurrentJob="عاطل",CurrentFaction="مدني"; public int Salary;
 public IReadOnlyDictionary<string,int> Salaries=>salaries;
 readonly Dictionary<string,int> salaries=new(){
  {"سائق شاحنة",900},{"سائق تاكسي",650},{"ميكانيكي",800},{"مسعف",1000},
  {"شرطي",1200},{"رجل إطفاء",1100},{"حارس أمن",850},{"تاجر",750},
  {"صياد",700},{"مزارع",600},{"عامل منجم",950},{"حلاق",650}
 };
 public bool ChooseJob(string job){if(!salaries.TryGetValue(job,out Salary))return false;CurrentJob=job;return true;}
 public bool JoinFaction(string faction){if(string.IsNullOrWhiteSpace(faction))return false;CurrentFaction=faction;return true;}
 public int CollectSalary(){return Salary;}
}
}