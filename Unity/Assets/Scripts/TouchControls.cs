using UnityEngine;
namespace CorruptStateRP {
public sealed class TouchControls:MonoBehaviour{
 public PlayerController player;public VehicleController vehicle;Vector2 move;bool run;
 public void SetMove(Vector2 v){move=Vector2.ClampMagnitude(v,1);}
 public void SetRun(bool v){run=v;}
 public void Update(){if(player&&player.gameObject.activeSelf)player.SetMoveInput(move,run);if(vehicle&&vehicle.Controlled)vehicle.Drive(move);}
 public void ClearMove(){move=Vector2.zero;run=false;}
}
}
