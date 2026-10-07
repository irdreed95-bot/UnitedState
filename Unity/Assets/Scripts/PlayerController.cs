using UnityEngine;
namespace CorruptStateRP {
[RequireComponent(typeof(CharacterController))]
public sealed class PlayerController:MonoBehaviour{
 public float walkSpeed=5.5f,runSpeed=8.5f,gravity=22f,jumpForce=7f;
 CharacterController cc; Vector2 input; bool running; float verticalVelocity;
 public bool WeaponEquipped{get;private set;} public bool Aiming{get;private set;} public bool InCover{get;private set;} public bool Sleeping{get;private set;} public int WeaponSlot{get;private set;}
 void Awake(){
  cc=GetComponent<CharacterController>();
  cc.height=1.8f; cc.radius=.32f; cc.center=new Vector3(0,.9f,0);
  var v=Resources.Load<GameObject>("Generated3D/citizen");
  if(v)Instantiate(v,transform);
  else BuildFallbackCitizen();
 }
 void BuildFallbackCitizen(){
  var root=new GameObject("CitizenVisual");
  root.transform.SetParent(transform,false);
  var skin=Mat(new Color(.62f,.43f,.31f)); var shirt=Mat(new Color(.10f,.16f,.24f)); var pants=Mat(new Color(.055f,.06f,.075f));
  Part(PrimitiveType.Capsule,new Vector3(0,1.02f,0),new Vector3(.62f,.82f,.62f),skin,root.transform);
  Part(PrimitiveType.Capsule,new Vector3(0,1.95f,0),new Vector3(.72f,.72f,.72f),skin,root.transform);
  Part(PrimitiveType.Cube,new Vector3(0,1.25f,0),new Vector3(.72f,.9f,.42f),shirt,root.transform);
  Part(PrimitiveType.Cube,new Vector3(-.24f,.47f,0),new Vector3(.25f,.78f,.34f),pants,root.transform);
  Part(PrimitiveType.Cube,new Vector3(.24f,.47f,0),new Vector3(.25f,.78f,.34f),pants,root.transform);
  Part(PrimitiveType.Cube,new Vector3(-.24f,.05f,.06f),new Vector3(.30f,.18f,.55f),pants,root.transform);
  Part(PrimitiveType.Cube,new Vector3(.24f,.05f,.06f),new Vector3(.30f,.18f,.55f),pants,root.transform);
  Part(PrimitiveType.Capsule,new Vector3(-.52f,1.32f,0),new Vector3(.22f,.62f,.22f),shirt,root.transform);
  Part(PrimitiveType.Capsule,new Vector3(.52f,1.32f,0),new Vector3(.22f,.62f,.22f),shirt,root.transform);
 }
 static void Part(PrimitiveType type,Vector3 pos,Vector3 scale,Material mat,Transform parent){
  var go=GameObject.CreatePrimitive(type); go.name="CitizenPart"; go.transform.SetParent(parent,false); go.transform.localPosition=pos; go.transform.localScale=scale; go.GetComponent<Renderer>().material=mat;
 }
 static Material Mat(Color c){var sh=Shader.Find("Universal Render Pipeline/Lit")??Shader.Find("Standard");var m=new Material(sh);m.color=c;return m;}
 public void SetMoveInput(Vector2 v,bool r){input=v;running=r&&!Sleeping;}
 public void Jump(){if(cc.isGrounded&&!Sleeping)verticalVelocity=jumpForce;}
 public void ToggleWeapon(){WeaponEquipped=!WeaponEquipped;if(!WeaponEquipped)Aiming=false;}
 public void SwitchWeapon(int slot){WeaponSlot=Mathf.Max(0,slot);WeaponEquipped=true;}
 public void ToggleAim(){if(WeaponEquipped&&!Sleeping)Aiming=!Aiming;}
 public void ToggleCover(){if(!Sleeping)InCover=!InCover;}
 public void ToggleSleep(){Sleeping=!Sleeping;if(Sleeping){Aiming=false;InCover=false;input=Vector2.zero;}}
 public void Fire(){if(WeaponEquipped&&!Sleeping)Debug.Log("Weapon fire slot "+WeaponSlot);}
 void Update(){
  var d=new Vector3(input.x,0,input.y); if(d.sqrMagnitude>1)d.Normalize();
  var v=d*(running?runSpeed:walkSpeed);
  if(cc.isGrounded&&verticalVelocity<0)verticalVelocity=-.5f;
  verticalVelocity-=gravity*Time.deltaTime; v.y=verticalVelocity; cc.Move(v*Time.deltaTime);
  if(d.sqrMagnitude>.01f&&!Sleeping)transform.rotation=Quaternion.Slerp(transform.rotation,Quaternion.LookRotation(d),Time.deltaTime*9);
 }
}
}