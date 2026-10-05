using System.Collections.Generic;using UnityEngine;
namespace CorruptStateRP {
[System.Serializable] public sealed class RPGameState {
 public int money=2500,bank=10000,xp,level=1,wanted;public float health=100,hunger=100,thirst=100,stamina=100;public string job="عاطل",faction="مدني";public int missionIndex,territoryProgress;public List<string> inventory=new(){"ماء","طعام","إسعاف","هوية"};
}
public sealed class SaveSystem:MonoBehaviour{
 public RPGameState State=new();
 const string Key="corrupt_state_unity_v1";
 public void Save(){PlayerPrefs.SetString(Key,JsonUtility.ToJson(State));PlayerPrefs.Save();}
 public void Load(){if(PlayerPrefs.HasKey(Key))State=JsonUtility.FromJson<RPGameState>(PlayerPrefs.GetString(Key))??new RPGameState();}
}
}
