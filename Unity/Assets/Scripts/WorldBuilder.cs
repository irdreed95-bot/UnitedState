using System;
using UnityEngine;

namespace CorruptStateRP {
public sealed class WorldBuilder : MonoBehaviour {
 public const float CitySize=680f;
 const float Cell=10f, RoadSpacing=40f;
 readonly System.Random rng=new System.Random(20261011);
 readonly string[] buildings={"Generated3D/commercial_a","Generated3D/commercial_b","Generated3D/skyscraper_a","Generated3D/skyscraper_b","Generated3D/skyscraper_c","Generated3D/suburban_a","Generated3D/suburban_b","Generated3D/suburban_c"};
 readonly (string,Vector3,string)[] landmarks={
  ("مركز الشرطة",new Vector3(-65,0,-65),"Generated3D/commercial_a"),
  ("المستشفى",new Vector3(65,0,-65),"Generated3D/commercial_b"),
  ("البنك المركزي",new Vector3(-65,0,65),"Generated3D/skyscraper_a"),
  ("دار الحكومة",new Vector3(65,0,65),"Generated3D/skyscraper_b"),
  ("المحكمة",new Vector3(-105,0,0),"Generated3D/suburban_a"),
  ("السجن",new Vector3(105,0,0),"Generated3D/suburban_b"),
  ("الكراج",new Vector3(0,0,-105),"Generated3D/commercial_a"),
  ("السوق",new Vector3(0,0,105),"Generated3D/commercial_b"),
  ("منطقة العصابات",new Vector3(-145,0,145),"Generated3D/suburban_c"),
  ("الميناء",new Vector3(215,0,165),"Generated3D/commercial_a")
 };
 Material asphalt,concrete,grass,water,white,yellow,glass,metal;
 public void Build(){MakeMaterials();Environment();GroundAndRoads();CoastAndHarbor();Parks();Buildings();Landmarks();Life();}
 void MakeMaterials(){
  asphalt=Mat(new Color(.075f,.085f,.095f));concrete=Mat(new Color(.42f,.43f,.42f));
  grass=Mat(new Color(.16f,.34f,.19f));water=Mat(new Color(.035f,.28f,.39f));
  white=Mat(new Color(.84f,.83f,.76f));yellow=Mat(new Color(.95f,.68f,.12f));
  glass=Mat(new Color(.18f,.34f,.43f));metal=Mat(new Color(.18f,.20f,.22f));
 }
 void Environment(){
  RenderSettings.ambientLight=new Color(.72f,.77f,.81f);RenderSettings.fog=true;
  RenderSettings.fogColor=new Color(.56f,.65f,.70f);RenderSettings.fogDensity=.0018f;
  var sun=new GameObject("CoastalCitySun").AddComponent<Light>();sun.type=LightType.Directional;
  sun.intensity=1.35f;sun.shadows=LightShadows.Soft;sun.transform.rotation=Quaternion.Euler(48,-32,0);
 }
 void GroundAndRoads(){
  var ground=Cube("CityTerrain",new Vector3(0,-.8f,0),new Vector3(CitySize,1,CitySize),grass);
  // Keep the terrain continuous; avoid thousands of small slab objects on mobile.
  for(int i=-30;i<=30;i++){
   float p=i*Cell;
   if(i%4==0||Mathf.Abs(i)<=1){
    Cube("Road_NS",new Vector3(p,-.05f,0),new Vector3(i%4==0?8f:10f,.16f,CitySize-50),asphalt);
    Cube("Road_EW",new Vector3(0,-.05f,p),new Vector3(CitySize-50,.16f,i%4==0?8f:10f),asphalt);
    if(i%4==0){LaneMark(new Vector3(p,-.0f,0),true);LaneMark(new Vector3(0,-.0f,p),false);}
   }
  }
  Cube("CentralPlaza",new Vector3(0,.03f,0),new Vector3(28,.12f,28),concrete);
  Cube("PlazaReflectingPool",new Vector3(0,.12f,0),new Vector3(8,.08f,8),water);
  for(int i=-13;i<=13;i++){
   StreetLight(new Vector3(i*20,0,24),Quaternion.identity);
   StreetLight(new Vector3(i*20,0,-24),Quaternion.identity);
   StreetLight(new Vector3(24,0,i*20),Quaternion.Euler(0,90,0));
   StreetLight(new Vector3(-24,0,i*20),Quaternion.Euler(0,90,0));
  }
 }
 void LaneMark(Vector3 p,bool vertical){
  for(int n=-14;n<=14;n+=2){Vector3 q=p+(vertical?new Vector3(0,.045f,n*20):new Vector3(n*20,.045f,0));Cube("LaneDash",q,vertical?new Vector3(.18f,.025f,4f):new Vector3(4f,.025f,.18f),yellow);}
 }
 void CoastAndHarbor(){
  Cube("Ocean",new Vector3(375,-.6f,0),new Vector3(110,.4f,740),water);
  Cube("SandyBeach",new Vector3(306,-.28f,0),new Vector3(26,.2f,680),Mat(new Color(.70f,.62f,.43f)));
  Cube("CoastalPromenade",new Vector3(286,-.05f,0),new Vector3(7,.18f,620),concrete);
  for(int z=-290;z<=290;z+=18){Tree(new Vector3(279,0,z),.8f);Bench(new Vector3(292,0,z+5));}
  Cube("HarborPier",new Vector3(240,.05f,160),new Vector3(72,.5f,15),Mat(new Color(.38f,.32f,.25f)));
  for(int x=215;x<=270;x+=18)for(int z=151;z<=169;z+=12)Cube("CargoContainer",new Vector3(x,2,z),new Vector3(14,4,9),Mat(new Color(.55f,.22f,.13f)));
  for(int x=225;x<=265;x+=20)Crane(new Vector3(x,0,183));
  for(int z=-120;z<=120;z+=40)Cube("HarborDockPost",new Vector3(251,0,z),new Vector3(1.2f,2,1.2f),metal);
 }
 void Parks(){
  Park(new Vector3(-210,0,-185),new Vector2(95,100),"RiversidePark");
  Park(new Vector3(-210,0,195),new Vector2(95,90),"WestPark");
  Park(new Vector3(130,0,-205),new Vector2(100,65),"BeachPark");
 }
 void Park(Vector3 c,Vector2 size,string name){
  Cube(name,c+Vector3.down*.15f,new Vector3(size.x,.16f,size.y),grass);
  for(float x=-size.x*.38f;x<=size.x*.38f;x+=size.x*.25f)
   for(float z=-size.y*.35f;z<=size.y*.35f;z+=size.y*.32f)Tree(c+new Vector3(x,0,z),.85f);
  Cube(name+"_Walkway",c+new Vector3(0,.02f,0),new Vector3(size.x*.8f,.08f,2.4f),concrete);
  for(int i=-2;i<=2;i++)Bench(c+new Vector3(i*size.x*.12f,0,5));
 }
 void Buildings(){
  for(int x=-28;x<=28;x+=2)for(int z=-28;z<=28;z+=2){
   float wx=x*Cell,wz=z*Cell;
   if(Mathf.Abs(x)<=1||Mathf.Abs(z)<=1||x%4==0||z%4==0)continue;
   if(wx>190&&Mathf.Abs(wz)>115)continue;
   if(InPark(wx,wz))continue;
   if(Mathf.Abs(wx)<25&&Mathf.Abs(wz)<25)continue;
   bool downtown=Mathf.Abs(wx)<125&&Mathf.Abs(wz)<125;
   string path=downtown?buildings[rng.Next(0,5)]:buildings[rng.Next(5,8)];
   var go=Spawn(path,new Vector3(wx,0,wz),downtown?1.2f:1.05f,"Building");
   if(go==null)BuildDetailedBuilding(new Vector3(wx,0,wz),downtown);
  }
 }
 bool InPark(float x,float z){
  return (x>-258&&x<-162&&z>-235&&z<-135)||(x>-258&&x<-162&&z>150&&z<240)||(x>80&&x<180&&z>-238&&z<-172);
 }
 GameObject BuildDetailedBuilding(Vector3 p,bool tall){
  var root=new GameObject(tall?"FallbackMidRise":"FallbackHouse");root.transform.position=p;
  float h=tall?rng.Next(5,12)*3f:rng.Next(2,4)*2.8f;
  float w=tall?rng.Next(9,15):rng.Next(7,10),d=tall?rng.Next(9,15):rng.Next(7,10);
  var body=CubeChild(root.transform,"Facade",new Vector3(0,h*.5f,0),new Vector3(w,h,d),Mat(tall?new Color(.27f,.31f,.34f):new Color(.58f,.48f,.37f)));
  for(int floor=0;floor<Mathf.FloorToInt(h/2.8f);floor++){
   for(int side=-1;side<=1;side+=2){
    var win=CubeChild(root.transform,"Window",new Vector3(side*(w*.5f+.025f),1.15f+floor*2.8f,0),new Vector3(.06f,1.25f,d*.62f),glass);
   }
  }
  CubeChild(root.transform,"RoofLip",new Vector3(0,h+.12f,0),new Vector3(w+.4f,.25f,d+.4f),metal);
  return root;
 }
 void Landmarks(){
  foreach(var l in landmarks){
   var go=Spawn(l.Item3,l.Item2,1.4f,l.Item1);
   if(go==null){go=BuildDetailedBuilding(l.Item2,true);go.name=l.Item1;}
   if(go==null)continue;
   var label=new GameObject("LandmarkLabel").AddComponent<TextMesh>();label.text=l.Item1;label.fontSize=48;label.characterSize=.08f;label.anchor=TextAnchor.MiddleCenter;label.color=Color.white;label.transform.SetParent(go.transform,false);label.transform.localPosition=new Vector3(0,12,0);
  }
 }
 void Life(){
  for(int i=0;i<90;i++){
   float x=(float)rng.NextDouble()*550-275,z=(float)rng.NextDouble()*550-275;
   if(Mathf.Abs(x)<125&&Mathf.Abs(z)<125)continue;
   if(Mathf.Abs(x%RoadSpacing)<8||Mathf.Abs(z%RoadSpacing)<8||InPark(x,z))continue;
   Tree(new Vector3(x,0,z),.75f+(float)rng.NextDouble()*.55f);
  }
  string[] cars={"Generated3D/sedan","Generated3D/suv","Generated3D/taxi","Generated3D/police"};
  for(int i=0;i<42;i++){
   float x=(float)rng.NextDouble()*510-255,z=(float)rng.NextDouble()*510-255;
   if(Mathf.Abs(x%RoadSpacing)>10&&Mathf.Abs(z%RoadSpacing)>10)continue;
   if(x>270||InPark(x,z))continue;
   Spawn(cars[rng.Next(cars.Length)],new Vector3(x,.15f,z),1,"ParkedVehicle");
  }
  for(int z=-250;z<=250;z+=45)for(int x=-250;x<=250;x+=45)
   if((x+z)%90==0&&!InPark(x,z))Bench(new Vector3(x,0,z));
 }
 void StreetLight(Vector3 p,Quaternion rot){
  var root=new GameObject("StreetLight");root.transform.SetPositionAndRotation(p,rot);
  var pole=GameObject.CreatePrimitive(PrimitiveType.Cylinder);pole.transform.SetParent(root.transform,false);pole.transform.localPosition=Vector3.up*3.1f;pole.transform.localScale=new Vector3(.09f,3.1f,.09f);pole.GetComponent<Renderer>().material=metal;
  var arm=CubeChild(root.transform,"LampArm",new Vector3(.5f,5.8f,0),new Vector3(1,.09f,.09f),metal);
  var lamp=GameObject.CreatePrimitive(PrimitiveType.Sphere);lamp.transform.SetParent(root.transform,false);lamp.transform.localPosition=new Vector3(1,5.7f,0);lamp.transform.localScale=Vector3.one*.3f;lamp.GetComponent<Renderer>().material=yellow;
  var light=lamp.AddComponent<Light>();light.type=LightType.Point;light.range=7;light.intensity=.55f;
 }
 void Crane(Vector3 p){
  var root=new GameObject("HarborCrane");root.transform.position=p;
  CubeChild(root.transform,"CraneMast",new Vector3(0,9,0),new Vector3(1,18,1),yellow);
  CubeChild(root.transform,"CraneBoom",new Vector3(0,17,5),new Vector3(1,1,14),yellow);
  CubeChild(root.transform,"CraneSupport",new Vector3(0,12,5),new Vector3(.5f,10,.5f),metal);
 }
 void Bench(Vector3 p){
  var root=new GameObject("StreetBench");root.transform.position=p;
  CubeChild(root.transform,"Seat",new Vector3(0,.55f,0),new Vector3(2,.15f,.55f),Mat(new Color(.30f,.20f,.12f)));
  CubeChild(root.transform,"Back",new Vector3(0,.95f,-.23f),new Vector3(2,.7f,.12f),Mat(new Color(.30f,.20f,.12f)));
  for(int x=-1;x<=1;x+=2)CubeChild(root.transform,"Leg",new Vector3(x*.75f,.25f,0),new Vector3(.12f,.5f,.12f),metal);
 }
 void Tree(Vector3 p,float s){
  var root=new GameObject("CityTree");root.transform.SetPositionAndRotation(p,Quaternion.identity);root.transform.localScale=Vector3.one*s;
  var trunk=GameObject.CreatePrimitive(PrimitiveType.Cylinder);trunk.transform.SetParent(root.transform,false);trunk.transform.localPosition=Vector3.up*1.25f;trunk.transform.localScale=new Vector3(.2f,1.25f,.2f);trunk.GetComponent<Renderer>().material=Mat(new Color(.34f,.22f,.13f));
  var crown=GameObject.CreatePrimitive(PrimitiveType.Sphere);crown.transform.SetParent(root.transform,false);crown.transform.localPosition=Vector3.up*3.1f;crown.transform.localScale=new Vector3(2.3f,2.7f,2.3f);crown.GetComponent<Renderer>().material=Mat(new Color(.15f,.37f,.19f));
 }
 GameObject Spawn(string path,Vector3 p,float s,string n){
  var prefab=Resources.Load<GameObject>(path);if(prefab==null)return null;
  var go=Instantiate(prefab,p,Quaternion.identity);go.name=n;go.transform.localScale*=s;return go;
 }
 GameObject Cube(string n,Vector3 p,Vector3 scale,Material m){
  var go=GameObject.CreatePrimitive(PrimitiveType.Cube);go.name=n;go.transform.position=p;go.transform.localScale=scale;go.GetComponent<Renderer>().material=m;return go;
 }
 GameObject CubeChild(Transform parent,string n,Vector3 p,Vector3 scale,Material m){
  var go=GameObject.CreatePrimitive(PrimitiveType.Cube);go.name=n;go.transform.SetParent(parent,false);go.transform.localPosition=p;go.transform.localScale=scale;go.GetComponent<Renderer>().material=m;return go;
 }
 static Material Mat(Color c){
  var shader=Shader.Find("Universal Render Pipeline/Lit");
  if(shader==null)shader=Shader.Find("Universal Render Pipeline/Simple Lit");
  if(shader==null)shader=Shader.Find("Universal Render Pipeline/Unlit");
  if(shader==null){Debug.LogError("No URP-compatible shader found; check URP package and pipeline asset.");shader=Shader.Find("Sprites/Default");}
  var m=new Material(shader);m.color=c;if(m.HasProperty("_Smoothness"))m.SetFloat("_Smoothness",.18f);return m;
 }
}
}