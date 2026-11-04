#if UNITY_EDITOR
using UnityEditor;using UnityEditor.SceneManagement;using UnityEngine;using UnityEditor.Build.Reporting;using UnityEngine.Rendering;using UnityEngine.Rendering.Universal;
namespace CorruptStateRP.Editor {
public static class BuildProject {
 [MenuItem("Corrupt State RP/Generate Onboarding Scenes")]
 public static void GenerateOnboardingScenes(){ConfigureProjectSettings();EnsureURP();SyncDataResources();Generate3DPrefabs();var gameplayScene=ResolveGameplayScene();EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene(gameplayScene,true)};AssetDatabase.SaveAssets();}
 static string ResolveGameplayScene(){
  const string importedCity="Assets/Scenes/demo_city_night.unity";
  const string generatedCity="Assets/Scenes/CityRPScene.unity";
  if(System.IO.File.Exists(importedCity)){Debug.Log("Using imported city scene: "+importedCity);return importedCity;}
  if(System.IO.File.Exists(generatedCity)){Debug.Log("Using existing generated gameplay scene: "+generatedCity);return generatedCity;}
  System.IO.Directory.CreateDirectory("Assets/Scenes");Make("CityRPScene",null);return generatedCity;
 }
 static void ConfigureProjectSettings(){
 PlayerSettings.companyName="Corrupt State Studio";PlayerSettings.productName="Corrupt State RP";PlayerSettings.bundleVersion="0.1.0";
 PlayerSettings.defaultInterfaceOrientation=UIOrientation.LandscapeLeft;PlayerSettings.allowedAutorotateToPortrait=false;PlayerSettings.allowedAutorotateToPortraitUpsideDown=false;PlayerSettings.allowedAutorotateToLandscapeRight=false;PlayerSettings.allowedAutorotateToLandscapeLeft=true;
 PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android,"com.corruptstate.rp");PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android,ScriptingImplementation.IL2CPP);PlayerSettings.SetApiCompatibilityLevel(BuildTargetGroup.Android,ApiCompatibilityLevel.NET_Standard);
 PlayerSettings.Android.minSdkVersion=AndroidSdkVersions.AndroidApiLevel26;PlayerSettings.Android.targetSdkVersion=AndroidSdkVersions.AndroidApiLevel35;PlayerSettings.Android.targetArchitectures=AndroidArchitecture.ARM64|AndroidArchitecture.ARMv7;PlayerSettings.Android.androidIsGame=true;EditorUserBuildSettings.buildAppBundle=false;
 } static void EnsureURP(){const string path="Assets/Settings/CorruptStateURP.asset";System.IO.Directory.CreateDirectory("Assets/Settings");var pipeline=AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(path);if(pipeline==null){var renderer=ScriptableObject.CreateInstance<UniversalRendererData>();AssetDatabase.CreateAsset(renderer,"Assets/Settings/CorruptStateRenderer.asset");pipeline=UniversalRenderPipelineAsset.Create(renderer);AssetDatabase.CreateAsset(pipeline,path);}GraphicsSettings.defaultRenderPipeline=pipeline;QualitySettings.renderPipeline=pipeline;
  var lit=Shader.Find("Universal Render Pipeline/Lit")??Shader.Find("Universal Render Pipeline/Simple Lit")??Shader.Find("Universal Render Pipeline/Unlit");
  if(lit==null)throw new System.Exception("URP shader not found; refusing to build a magenta world.");
  const string materialPath="Assets/Settings/RuntimeLit.mat";
  var material=AssetDatabase.LoadAssetAtPath<Material>(materialPath);
  if(material==null){material=new Material(lit);AssetDatabase.CreateAsset(material,materialPath);}else material.shader=lit;
  AssetDatabase.SaveAssets();}
 static void SyncDataResources(){System.IO.Directory.CreateDirectory("Assets/Resources/Data");foreach(var name in new[]{"factions.json","jobs.json","streets.json"}){var source="Assets/Data/"+name;var target="Assets/Resources/Data/"+name;if(!System.IO.File.Exists(source))throw new System.Exception("Required data file missing: "+source);System.IO.File.Copy(source,target,true);}AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);}
 static void Generate3DPrefabs(){AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);var dir="Assets/Resources/Generated3D";System.IO.Directory.CreateDirectory(dir);string[] models={"Assets/Resources/External/city/commercial_a.glb","Assets/Resources/External/city/commercial_b.glb","Assets/Resources/External/city/commercial_c.glb","Assets/Resources/External/city/commercial_d.glb","Assets/Resources/External/city/commercial_e.glb","Assets/Resources/External/city/commercial_f.glb","Assets/Resources/External/city/skyscraper_a.glb","Assets/Resources/External/city/skyscraper_b.glb","Assets/Resources/External/city/skyscraper_c.glb","Assets/Resources/External/city/suburban_a.glb","Assets/Resources/External/city/suburban_b.glb","Assets/Resources/External/city/suburban_c.glb","Assets/Resources/External/city/suburban_d.glb","Assets/Resources/External/city/suburban_e.glb","Assets/Resources/External/city/suburban_f.glb","Assets/Resources/External/vehicles/sedan.glb","Assets/Resources/External/vehicles/suv.glb","Assets/Resources/External/vehicles/taxi.glb","Assets/Resources/External/vehicles/police.glb","Assets/Resources/External/vehicles/ambulance.glb","Assets/Resources/External/vehicles/firetruck.glb","Assets/Resources/External/vehicles/van.glb","Assets/Resources/External/character/citizen.glb"};foreach(var model in models){if(!System.IO.File.Exists(model))throw new System.Exception("Required 3D model file is missing: "+model);AssetDatabase.ImportAsset(model,ImportAssetOptions.ForceUpdate|ImportAssetOptions.ForceSynchronousImport);var source=AssetDatabase.LoadAssetAtPath<GameObject>(model);if(source==null)throw new System.Exception("Required 3D model failed to import after URP setup: "+model);if(source.GetComponentsInChildren<Renderer>(true).Length==0)throw new System.Exception("Imported 3D model contains no renderable mesh: "+model);var name=System.IO.Path.GetFileNameWithoutExtension(model);var instance=Object.Instantiate(source);instance.name=name;NormalizeMaterials(instance);bool saved;PrefabUtility.SaveAsPrefabAsset(instance,dir+"/"+name+".prefab",out saved);Object.DestroyImmediate(instance);if(!saved)throw new System.Exception("Failed to save generated prefab: "+name);Debug.Log("Imported and validated URP-ready 3D prefab: "+name);}AssetDatabase.SaveAssets();AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);}
 static void NormalizeMaterials(GameObject root){
  var lit=Shader.Find("Universal Render Pipeline/Lit")??Shader.Find("Universal Render Pipeline/Simple Lit");
  if(lit==null)throw new System.Exception("Cannot convert model materials: no URP shader available.");
  const string materialDir="Assets/Resources/Generated3D/Materials";
  System.IO.Directory.CreateDirectory(materialDir);
  int materialIndex=0;
  foreach(var renderer in root.GetComponentsInChildren<Renderer>(true)){
   var mats=renderer.sharedMaterials;
   for(int i=0;i<mats.Length;i++){
    var old=mats[i];if(old==null)throw new System.Exception("Null material in imported model: "+root.name);
    Color sourceColor=old.HasProperty("_BaseColor")?old.GetColor("_BaseColor"):(old.HasProperty("_Color")?old.color:Color.white);
    if(sourceColor.r>.75f&&sourceColor.b>.75f&&sourceColor.g<.3f)sourceColor=Color.gray;
    var replacement=new Material(lit){name=root.name+"_"+old.name+"_"+materialIndex,color=sourceColor};
    if(replacement.HasProperty("_BaseColor"))replacement.SetColor("_BaseColor",sourceColor);
    CopyTexture(old,replacement,"_BaseMap","_BaseMap","_MainTex","_BaseColorTexture");
    CopyTexture(old,replacement,"_BumpMap","_BumpMap","_NormalMap","_NormalTexture");
    CopyTexture(old,replacement,"_MetallicGlossMap","_MetallicGlossMap","_MetallicRoughnessMap","_MetallicRoughnessTexture");
    CopyTexture(old,replacement,"_OcclusionMap","_OcclusionMap","_OcclusionTexture");
    CopyTexture(old,replacement,"_EmissionMap","_EmissionMap","_EmissiveTexture");
    if(replacement.HasProperty("_Smoothness"))replacement.SetFloat("_Smoothness",old.HasProperty("_Smoothness")?old.GetFloat("_Smoothness"):.22f);
    if(replacement.HasProperty("_BumpMap")&&replacement.GetTexture("_BumpMap")!=null)replacement.EnableKeyword("_NORMALMAP");
    if(replacement.HasProperty("_EmissionColor")&&old.HasProperty("_EmissiveFactor"))replacement.SetColor("_EmissionColor",old.GetColor("_EmissiveFactor"));
    var safeName=(root.name+"_"+old.name+"_"+materialIndex).Replace("/","_").Replace("\\","_");
    var assetPath=AssetDatabase.GenerateUniqueAssetPath(materialDir+"/"+safeName+".mat");
    AssetDatabase.CreateAsset(replacement,assetPath);
    mats[i]=AssetDatabase.LoadAssetAtPath<Material>(assetPath);
    materialIndex++;
   }
   renderer.sharedMaterials=mats;
  }
 }
 static void CopyTexture(Material source,Material target,string targetProperty,params string[] sourceProperties){
  if(!target.HasProperty(targetProperty))return;
  foreach(var property in sourceProperties){if(source.HasProperty(property)){var texture=source.GetTexture(property);if(texture!=null){target.SetTexture(targetProperty,texture);return;}}}
 }
 static void Make(string name,CorruptStateRP.Onboarding.OnboardingScreen.Mode? mode){var s=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);if(mode.HasValue){var go=new GameObject("OnboardingScreen");var c=go.AddComponent<CorruptStateRP.Onboarding.OnboardingScreen>();var so=new SerializedObject(c);so.FindProperty("mode").enumValueIndex=(int)mode.Value;so.ApplyModifiedPropertiesWithoutUndo();if(mode.Value==CorruptStateRP.Onboarding.OnboardingScreen.Mode.Lobby){go.AddComponent<CorruptStateRP.Lobby.LobbyScreen>();}if(mode.Value==CorruptStateRP.Onboarding.OnboardingScreen.Mode.Splash){new GameObject("AuthClient").AddComponent<CorruptStateRP.Auth.AuthClient>();go.AddComponent<CorruptStateRP.Onboarding.OnboardingFlowController>();}}else{var gm=new GameObject("GameManager");gm.AddComponent<CorruptStateRP.GameManager>();gm.AddComponent<CorruptStateRP.CityInteractionSystem>();var mobile=new GameObject("MobileHUD");mobile.AddComponent<CorruptStateRP.Mobile.MobileHUD>();}EditorSceneManager.SaveScene(s,"Assets/Scenes/"+name+".unity");}
 [MenuItem("Corrupt State RP/Build Android")]
 public static void BuildAndroid(){GenerateOnboardingScenes();var format=(System.Environment.GetEnvironmentVariable("CORRUPTSTATE_BUILD_FORMAT")??"apk").Trim().ToLowerInvariant();var buildAab=format=="aab";if(!buildAab&&format!="apk")throw new System.Exception("CORRUPTSTATE_BUILD_FORMAT must be apk or aab; got: "+format);EditorUserBuildSettings.buildAppBundle=buildAab;var extension=buildAab?".aab":".apk";var output="Builds/Android/CorruptStateRP"+extension;System.IO.Directory.CreateDirectory("Builds/Android");var gameplayScene=ResolveGameplayScene();EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene(gameplayScene,true)};var o=new BuildPlayerOptions{scenes=new[]{gameplayScene},locationPathName=output,target=BuildTarget.Android,options=BuildOptions.None};Debug.Log("CORRUPTSTATE_BUILD_FORMAT="+format+"; locationPathName="+o.locationPathName+"; buildAppBundle="+EditorUserBuildSettings.buildAppBundle);var r=BuildPipeline.BuildPlayer(o);if(r.summary.result!=BuildResult.Succeeded)throw new System.Exception("Android "+format.ToUpperInvariant()+" build failed: "+r.summary.result);if(!System.IO.File.Exists(output))throw new System.Exception("Build reported success but output file is missing: "+output);Debug.Log("Android build output verified: "+output+" ("+new System.IO.FileInfo(output).Length+" bytes)");}
}}
#endif