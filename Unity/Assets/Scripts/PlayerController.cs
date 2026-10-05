using UnityEngine;
namespace CorruptStateRP {
[RequireComponent(typeof(CharacterController))]
public sealed class PlayerController:MonoBehaviour{
 public float walkSpeed=5.5f,runSpeed=8.5f,gravity=22f; CharacterController cc; Vector2 input; bool running;
 void Awake(){cc=GetComponent<CharacterController>();var v=Resources.Load<GameObject>("External/character/citizen");if(v)Instantiate(v,transform);}
 public void SetMoveInput(Vector2 v,bool r){input=v;running=r;}
 void Update(){var d=new Vector3(input.x,0,input.y);if(d.sqrMagnitude>1)d.Normalize();var v=d*(running?runSpeed:walkSpeed);v.y=cc.isGrounded?-.2f:-gravity;cc.Move(v*Time.deltaTime);if(d.sqrMagnitude>.01f)transform.rotation=Quaternion.Slerp(transform.rotation,Quaternion.LookRotation(d),Time.deltaTime*9);}
}}
