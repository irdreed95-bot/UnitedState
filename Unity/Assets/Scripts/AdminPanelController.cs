using UnityEngine;
namespace CorruptStateRP {
 public sealed class AdminPanelController:MonoBehaviour {
  public void SearchPlayer(string nameOrId){Debug.Log("Admin search: "+nameOrId);}
  public void FreeCam(bool enabled){Debug.Log("Free cam: "+enabled);}
  public void Jail(string playerId){Debug.Log("Jail: "+playerId);}
  public void Unjail(string playerId){Debug.Log("Unjail: "+playerId);}
  public void Mute(string playerId){Debug.Log("Mute: "+playerId);}
  public void Kick(string playerId){Debug.Log("Kick: "+playerId);}
  public void Ban(string playerId){Debug.Log("Ban: "+playerId);}
  public void TeleportTo(string playerId){Debug.Log("Teleport to: "+playerId);}
  public void BringPlayer(string playerId){Debug.Log("Bring: "+playerId);}
  public void FlipVehicle(string vehicleId){Debug.Log("Flip vehicle: "+vehicleId);}
  public void ShowOnlineAdmins(){Debug.Log("Show online administrators");}
  public void ShowKillList(){Debug.Log("Show last five kill events");}
  public void TeleportToMarker(){Debug.Log("Teleport to marker");}
 }
}