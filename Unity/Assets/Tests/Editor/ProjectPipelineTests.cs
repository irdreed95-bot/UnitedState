using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
public sealed class ProjectPipelineTests {
 static readonly string[] RequiredPrefabs={
  "commercial_a","commercial_b","commercial_c","commercial_d","commercial_e","commercial_f",
  "skyscraper_a","skyscraper_b","skyscraper_c",
  "suburban_a","suburban_b","suburban_c","suburban_d","suburban_e","suburban_f",
  "sedan","suv","taxi","police","ambulance","firetruck","van","citizen"
 };
 [Test]
 public void GenerateAndValidateWorldAssetsWithoutBuildingAnApk(){
  var builder=AppDomain.CurrentDomain.GetAssemblies().Select(a=>a.GetType("CorruptStateRP.Editor.BuildProject",false)).FirstOrDefault(t=>t!=null);
  Assert.NotNull(builder,"BuildProject editor class did not compile.");
  var generate=builder.GetMethod("GenerateOnboardingScenes",BindingFlags.Public|BindingFlags.Static);
  Assert.NotNull(generate,"GenerateOnboardingScenes method is missing.");
  try{generate.Invoke(null,null);}catch(TargetInvocationException e){throw e.InnerException??e;}
  AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
  foreach(var name in RequiredPrefabs){
   var path="Assets/Resources/Generated3D/"+name+".prefab";
   var prefab=AssetDatabase.LoadAssetAtPath<GameObject>(path);
   Assert.NotNull(prefab,"Required 3D prefab was not generated: "+path);
   var renderers=prefab.GetComponentsInChildren<Renderer>(true);
   Assert.Greater(renderers.Length,0,"3D prefab has no visible renderer: "+name);
   foreach(var renderer in renderers)foreach(var material in renderer.sharedMaterials){
    Assert.NotNull(material,"Null material in "+name);
    Assert.NotNull(material.shader,"Missing shader in "+name+"/"+material.name);
    Assert.IsFalse(material.shader.name.Contains("InternalError"),"Unity error shader found in "+name+"/"+material.name);
   }
  }
  var catalog=AppDomain.CurrentDomain.GetAssemblies().Select(a=>a.GetType("CorruptStateRP.GameDataCatalog",false)).FirstOrDefault(t=>t!=null);
  Assert.NotNull(catalog,"GameDataCatalog did not compile.");
  var load=catalog.GetMethod("LoadAll",BindingFlags.Public|BindingFlags.Static);
  Assert.NotNull(load);
  try{load.Invoke(null,null);}catch(TargetInvocationException e){throw e.InnerException??e;}
  Assert.AreEqual(4,EditorBuildSettings.scenes.Length,"Expected Splash, Login, Lobby and Gameplay scenes.");
  string[] expectedScenes={"Assets/Scenes/SplashScene.unity","Assets/Scenes/LoginScene.unity","Assets/Scenes/LobbyScene.unity","Assets/Scenes/CityRPScene.unity"};
  for(int i=0;i<expectedScenes.Length;i++){Assert.IsTrue(EditorBuildSettings.scenes[i].enabled,"Scene is not enabled: "+expectedScenes[i]);Assert.AreEqual(expectedScenes[i],EditorBuildSettings.scenes[i].path,"Scene order mismatch at index "+i);Assert.IsTrue(AssetDatabase.LoadAssetAtPath<SceneAsset>(expectedScenes[i])!=null,"Scene asset is missing: "+expectedScenes[i]);}
  var screenType=AppDomain.CurrentDomain.GetAssemblies().Select(a=>a.GetType("CorruptStateRP.Onboarding.OnboardingScreen",false)).FirstOrDefault(t=>t!=null);
  Assert.NotNull(screenType,"OnboardingScreen did not compile.");
  for(int i=0;i<3;i++){
   var scene=EditorSceneManager.OpenScene(expectedScenes[i],OpenSceneMode.Single);
   var screenObject=GameObject.Find("OnboardingScreen");Assert.NotNull(screenObject,"Missing onboarding root in "+expectedScenes[i]);
   var screen=screenObject.GetComponent(screenType);Assert.NotNull(screen);
   var serialized=new SerializedObject(screen);Assert.AreEqual(i,serialized.FindProperty("mode").enumValueIndex,"Incorrect onboarding mode in "+expectedScenes[i]);
  }
  EditorSceneManager.OpenScene(expectedScenes[3],OpenSceneMode.Single);
  Assert.NotNull(GameObject.Find("GameManager"),"Gameplay scene has no GameManager.");
  Assert.NotNull(GameObject.Find("MobileHUD"),"Gameplay scene has no mobile HUD.");

 }
}
