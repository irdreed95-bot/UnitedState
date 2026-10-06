using UnityEngine;
namespace CorruptStateRP {
 public sealed class FPSManager : MonoBehaviour {
  [SerializeField] int defaultFPS=60; public int TargetFPS{get;private set;}
  void Awake(){QualitySettings.vSyncCount=0;SetFrameRate(defaultFPS);}
  public void SetFrameRate(int targetFPS){TargetFPS=Mathf.Clamp(targetFPS,20,120);Application.targetFrameRate=TargetFPS;}
 }
}