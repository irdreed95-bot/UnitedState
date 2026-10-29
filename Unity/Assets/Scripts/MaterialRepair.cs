using UnityEngine;
using UnityEngine.Rendering;

namespace CorruptStateRP {
 public static class MaterialRepair {
  public static void Apply(GameObject root) {
   if(root==null)return;
   var shader=Shader.Find("Universal Render Pipeline/Lit")??Shader.Find("Universal Render Pipeline/Simple Lit")??Shader.Find("Universal Render Pipeline/Unlit")??Shader.Find("Standard");
   if(shader==null){Debug.LogError("MaterialRepair: no supported shader found.");return;}
   foreach(var renderer in root.GetComponentsInChildren<Renderer>(true)){
    var materials=renderer.sharedMaterials;
    for(int i=0;i<materials.Length;i++){
     var old=materials[i];
     if(old==null){materials[i]=new Material(shader){color=Color.gray};continue;}
     Color tint=Color.white;
     if(old.HasProperty("_BaseColor"))tint=old.GetColor("_BaseColor");
     else if(old.HasProperty("_Color"))tint=old.color;
     bool magenta=tint.r>.75f&&tint.b>.75f&&tint.g<.3f;
     if(magenta)tint=Color.gray;
     var fixedMat=new Material(shader){name=old.name+" (Runtime Fixed)",color=tint};
     if(fixedMat.HasProperty("_BaseMap")){
      Texture tex=old.HasProperty("_BaseMap")?old.GetTexture("_BaseMap"):old.HasProperty("_MainTex")?old.GetTexture("_MainTex"):null;
      if(tex!=null)fixedMat.SetTexture("_BaseMap",tex);
     }
     if(fixedMat.HasProperty("_BaseColor"))fixedMat.SetColor("_BaseColor",tint);
     if(fixedMat.HasProperty("_Smoothness"))fixedMat.SetFloat("_Smoothness",.22f);
     materials[i]=fixedMat;
    }
    renderer.sharedMaterials=materials;
   }
  }
 }
}