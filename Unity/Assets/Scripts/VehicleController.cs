using UnityEngine;
namespace CorruptStateRP {
public sealed class VehicleController:MonoBehaviour{
 public float acceleration=12,maxSpeed=24,turnSpeed=55;
 public bool EngineOn{get;private set;} public bool Controlled{get;private set;}
 public bool LightsOn{get;private set;} public bool HazardsOn{get;private set;} public bool SeatbeltOn{get;private set;} public bool WipersOn{get;private set;}
 public bool FrontLeftDoorOpen{get;private set;}
 Rigidbody rb;
 public void Initialize(){rb=gameObject.AddComponent<Rigidbody>();rb.mass=1100;rb.centerOfMass=new(0,-.4f,0);var c=gameObject.AddComponent<BoxCollider>();c.size=new(1.9f,.9f,4);var p=Resources.Load<GameObject>("Generated3D/sedan");if(p)Instantiate(p,transform);else{var b=GameObject.CreatePrimitive(PrimitiveType.Cube);b.transform.SetParent(transform,false);b.transform.localScale=new(1.9f,.6f,4);b.transform.localPosition=Vector3.up*.6f;}}
 public void SetControlled(bool v){Controlled=v;if(!v&&rb)rb.linearVelocity=Vector3.zero;}
 public void ToggleEngine(){EngineOn=!EngineOn;if(!EngineOn&&rb)rb.linearVelocity=Vector3.zero;}
 public void ToggleLights(){LightsOn=!LightsOn;}
 public void ToggleHazards(){HazardsOn=!HazardsOn;}
 public void ToggleSeatbelt(){SeatbeltOn=!SeatbeltOn;}
 public void ToggleWipers(){WipersOn=!WipersOn;}
 public void ToggleDoor(){FrontLeftDoorOpen=!FrontLeftDoorOpen;}
 public void Drive(Vector2 i){if(!Controlled||!EngineOn||rb==null)return;float t=-i.y;rb.AddForce(transform.forward*t*acceleration,ForceMode.Acceleration);var l=transform.InverseTransformDirection(rb.linearVelocity);l.z=Mathf.Clamp(l.z,-maxSpeed,maxSpeed);rb.linearVelocity=transform.TransformDirection(l);transform.Rotate(0,i.x*turnSpeed*Time.deltaTime*Mathf.Clamp01(rb.linearVelocity.magnitude/3),0);}
}}
