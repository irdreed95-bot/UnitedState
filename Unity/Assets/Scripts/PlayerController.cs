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
  if(v==null)throw new System.InvalidOperationException("Required citizen prefab is missing: Resources/Generated3D/citizen");
  Instantiate(v,transform);
 }
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