using UnityEngine;
namespace CorruptStateRP {
public sealed class InputRouter:MonoBehaviour {
 public static Vector2 Move;public static bool Sprint,Jump,Fire,Aim,Cover,Sleep;
 PlayerController player;
 public void Bind(PlayerController p){player=p;}
 public void SetMove(Vector2 v){Move=Vector2.ClampMagnitude(v,1f);}public void SetSprint(bool v){Sprint=v;}
 public void DoJump(){Jump=true;}public void DoFire(){Fire=true;}public void DoAim(){Aim=!Aim;player?.ToggleAim();}public void DoCover(){Cover=!Cover;player?.ToggleCover();}public void DoSleep(){Sleep=!Sleep;player?.ToggleSleep();}
 void Update(){if(player!=null){if(Jump)player.Jump();if(Fire)player.Fire();}Jump=false;Fire=false;}
}}