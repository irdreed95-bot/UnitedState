using UnityEngine;
namespace CorruptStateRP{
public sealed class InputRouter:MonoBehaviour{
 public static Vector2 Move; public static bool Sprint;
 public static bool Jump; public static bool Fire; public static bool Aim; public static bool Cover; public static bool Sleep;
 public void SetMove(Vector2 v){Move=Vector2.ClampMagnitude(v,1f);}
 public void SetSprint(bool v){Sprint=v;}
 public void DoJump(){Jump=true;}
 public void DoFire(){Fire=true;}
 public void DoAim(){Aim=!Aim;}
 public void DoCover(){Cover=!Cover;}
 public void DoSleep(){Sleep=!Sleep;}
 public static void ConsumeFrame(){Jump=false;Fire=false;}
}}
