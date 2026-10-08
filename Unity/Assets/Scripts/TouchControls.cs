using UnityEngine;
namespace CorruptStateRP {
public sealed class TouchControls:MonoBehaviour {
 public PlayerController player;public VehicleController vehicle;Vector2 move;bool run;
 public void SetMove(Vector2 v){move=Vector2.ClampMagnitude(v,1);InputRouter.Move=move;}
 public void SetRun(bool v){run=v;InputRouter.Sprint=v;}
 public void Update(){InputRouter.Move=move;InputRouter.Sprint=run;}
 public void ClearMove(){move=Vector2.zero;run=false;InputRouter.Move=Vector2.zero;InputRouter.Sprint=false;}
}}