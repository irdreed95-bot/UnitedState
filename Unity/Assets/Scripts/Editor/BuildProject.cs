#if UNITY_EDITOR
using UnityEditor;using UnityEditor.SceneManagement;using UnityEngine;using UnityEditor.Build.Reporting;
namespace CorruptStateRP.Editor {
public static class BuildProject{
 [MenuItem("Corrupt State RP/Generate Main Scene")]
 public static void GenerateMainScene(){var s=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);var g=new GameObject("GameManager");g.AddComponent<GameManager>();System.IO.Directory.CreateDirectory("Assets/Scenes");EditorSceneManager.SaveScene(s,"Assets/Scenes/Main.unity");EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene("Assets/Scenes/Main.unity",true)};AssetDatabase.SaveAssets();}
 public static void BuildAndroid(){GenerateMainScene();var o=new BuildPlayerOptions{scenes=new[]{"Assets/Scenes/Main.unity"},locationPathName="Builds/Android/CorruptStateRP.apk",target=BuildTarget.Android,options=BuildOptions.None};var r=BuildPipeline.BuildPlayer(o);if(r.summary.result!=BuildResult.Succeeded)throw new System.Exception("Android build failed: "+r.summary.result);}
}}
#endif
