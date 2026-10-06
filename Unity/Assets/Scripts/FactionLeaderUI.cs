using UnityEngine;
namespace CorruptStateRP {
 public sealed class FactionLeaderUI:MonoBehaviour {
  public void GrantLeave(int days){Debug.Log("Grant leave: "+days+" days");}
  public void DemoteMember(string playerId){Debug.Log("Demote: "+playerId);}
  public void IssueWarning(string playerId){Debug.Log("Warning: "+playerId);}
  public void KickMember(string playerId){Debug.Log("Kick: "+playerId);}
  public void PromoteMember(string playerId){Debug.Log("Promote: "+playerId);}
  public void AssignDeputy(string playerId){Debug.Log("Assign deputy: "+playerId);}
  public void RewardMember(string playerId,int amount,int xp){Debug.Log("Reward "+playerId+": $"+amount+", XP "+xp);}
 }
}