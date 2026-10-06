#if UNITY_EDITOR
using UnityEditor;using UnityEditor.SceneManagement;using UnityEngine;using UnityEditor.Build.Reporting;
namespace CorruptStateRP.Editor {
public static class BuildProject{
 [MenuItem("Corrupt State RP/Generate Onboarding Scenes")]
 public static void GenerateOnboardingScenes(){System.IO.Directory.CreateDirectory("Assets/Scenes");Make("SplashScreenScene",CorruptStateRP.Onboarding.OnboardingScreen.Mode.Splash);Make("LoginScene",CorruptStateRP.Onboarding.OnboardingScreen.Mode.Login);Make("LobbyScene",CorruptStateRP.Onboarding.OnboardingScreen.Mode.Lobby);Make("CityRPScene",null);EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene("Assets/Scenes/SplashScreenScene.unity",true),new EditorBuildSettingsScene("Assets/Scenes/LoginScene.unity",true),new EditorBuildSettingsScene("Assets/Scenes/LobbyScene.unity",true),new EditorBuildSettingsScene("Assets/Scenes/CityRPScene.unity",true)};AssetDatabase.SaveAssets();}
 static void Make(string name,CorruptStateRP.Onboarding.OnboardingScreen.Mode? mode){var s=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);if(mode.HasValue){var go=new GameObject("OnboardingScreen");var c=go.AddComponent<CorruptStateRP.Onboarding.OnboardingScreen>();var so=new SerializedObject(c);so.FindProperty("mode").enumValueIndex=(int)mode.Value;so.ApplyModifiedPropertiesWithoutUndo();}EditorSceneManager.SaveScene(s,"Assets/Scenes/"+name+".unity");}
 [MenuItem("Corrupt State RP/Build Android")]
 public static void BuildAndroid(){GenerateOnboardingScenes();var o=new BuildPlayerOptions{scenes=new[]{"Assets/Scenes/SplashScreenScene.unity","Assets/Scenes/LoginScene.unity","Assets/Scenes/LobbyScene.unity","Assets/Scenes/CityRPScene.unity"},locationPathName="Builds/Android/CorruptStateRP.apk",target=BuildTarget.Android,options=BuildOptions.None};var r=BuildPipeline.BuildPlayer(o);if(r.summary.result!=BuildResult.Succeeded)throw new System.Exception("Android build failed: "+r.summary.result);}
}}
#endif