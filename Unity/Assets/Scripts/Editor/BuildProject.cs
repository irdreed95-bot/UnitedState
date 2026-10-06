#if UNITY_EDITOR
using UnityEditor;using UnityEditor.SceneManagement;using UnityEngine;using UnityEditor.Build.Reporting;
namespace CorruptStateRP.Editor {
public static class BuildProject{
 [MenuItem("Corrupt State RP/Generate Onboarding Scenes")]
 public static void GenerateOnboardingScenes(){
  PlayerSettings.defaultInterfaceOrientation=UIOrientation.LandscapeLeft;
  PlayerSettings.allowedAutorotateToPortrait=false;
  PlayerSettings.allowedAutorotateToPortraitUpsideDown=false;
  PlayerSettings.allowedAutorotateToLandscapeRight=false;
  PlayerSettings.allowedAutorotateToLandscapeLeft=true;
  System.IO.Directory.CreateDirectory("Assets/Scenes");
  Make("SplashScreenScene",CorruptStateRP.Onboarding.OnboardingScreen.Mode.Splash);
  Make("LoginScene",CorruptStateRP.Onboarding.OnboardingScreen.Mode.Login);
  Make("LobbyScene",CorruptStateRP.Onboarding.OnboardingScreen.Mode.Lobby);
  Make("CityRPScene",null);
  EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene("Assets/Scenes/SplashScreenScene.unity",true),new EditorBuildSettingsScene("Assets/Scenes/LoginScene.unity",true),new EditorBuildSettingsScene("Assets/Scenes/LobbyScene.unity",true),new EditorBuildSettingsScene("Assets/Scenes/CityRPScene.unity",true)};
  AssetDatabase.SaveAssets();
 }
 static void Make(string name,CorruptStateRP.Onboarding.OnboardingScreen.Mode? mode){
  var s=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
  if(mode.HasValue){
   var go=new GameObject("OnboardingScreen");
   var c=go.AddComponent<CorruptStateRP.Onboarding.OnboardingScreen>();
   var so=new SerializedObject(c);
   so.FindProperty("mode").enumValueIndex=(int)mode.Value;
   so.ApplyModifiedPropertiesWithoutUndo();
   if(mode.Value==CorruptStateRP.Onboarding.OnboardingScreen.Mode.Lobby) go.AddComponent<CorruptStateRP.Lobby.LobbyScreen>();
   if(mode.Value==CorruptStateRP.Onboarding.OnboardingScreen.Mode.Splash){
    var auth=go.AddComponent<CorruptStateRP.Auth.AuthClient>();
    go.AddComponent<CorruptStateRP.Onboarding.OnboardingFlowController>();
   }
  }else{
   var gm=new GameObject("GameManager");
   gm.AddComponent<CorruptStateRP.GameManager>();
   gm.AddComponent<CorruptStateRP.CityInteractionSystem>();
   gm.AddComponent<CorruptStateRP.FactionDashboard>();
   gm.AddComponent<CorruptStateRP.AdminDashboard>();
   gm.AddComponent<CorruptStateRP.AdminPanelController>();
   gm.AddComponent<CorruptStateRP.FactionLeaderUI>();
   var mobile=new GameObject("MobileHUD");
   mobile.AddComponent<CorruptStateRP.Mobile.MobileHUD>();
   var citizen=new GameObject("NewCitizenProgram");
   citizen.AddComponent<CorruptStateRP.Citizens.NewCitizenProgram>();
  }
  EditorSceneManager.SaveScene(s,"Assets/Scenes/"+name+".unity");
 }
 [MenuItem("Corrupt State RP/Build Android")]
 public static void BuildAndroid(){
  GenerateOnboardingScenes();
  PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
  PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64 | AndroidArchitecture.ARMv7;
  PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel26;
  PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel35;
  PlayerSettings.Android.androidIsGame = true;

  var o=new BuildPlayerOptions{scenes=new[]{"Assets/Scenes/SplashScreenScene.unity","Assets/Scenes/LoginScene.unity","Assets/Scenes/LobbyScene.unity","Assets/Scenes/CityRPScene.unity"},locationPathName="Builds/Android/CorruptStateRP.apk",target=BuildTarget.Android,options=BuildOptions.None};
  var r=BuildPipeline.BuildPlayer(o);
  if(r.summary.result!=BuildResult.Succeeded)throw new System.Exception("Android build failed: "+r.summary.result);
 }
}}
#endif
