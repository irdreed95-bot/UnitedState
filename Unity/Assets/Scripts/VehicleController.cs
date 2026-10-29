using UnityEngine;
namespace CorruptStateRP {
public sealed class VehicleController:MonoBehaviour{
 public float acceleration=12,maxSpeed=24,turnSpeed=55;
 public bool EngineOn{get;private set;} public bool Controlled{get;private set;}
 public bool LightsOn{get;private set;} public bool HazardsOn{get;private set;} public bool SeatbeltOn{get;private set;} public bool WipersOn{get;private set;}
 public bool FrontLeftDoorOpen{get;private set;}
 Rigidbody rb;
 public void Initialize(){rb=gameObject.AddComponent<Rigidbody>();rb.mass=1100;rb.centerOfMass=new(0,-.4f,0);var c=gameObject.AddComponent<BoxCollider>();c.size=new(1.9f,.9f,4);var p=Resources.Load<GameObject>("Generated3D/sedan");if(p){var model=Instantiate(p,transform);model.name="VehicleBody";MaterialRepair.Apply(model);}else{BuildFallbackCar();}}
 void BuildFallbackCar(){
  var body=GameObject.CreatePrimitive(PrimitiveType.Cube);body.name="CarBody";body.transform.SetParent(transform,false);body.transform.localScale=new Vector3(1.85f,.55f,3.7f);body.transform.localPosition=new Vector3(0,.62f,0);body.GetComponent<Renderer>().material=new Material(Shader.Find("Universal Render Pipeline/Lit")??Shader.Find("Standard")){color=new Color(.12f,.28f,.48f)};
  var cabin=GameObject.CreatePrimitive(PrimitiveType.Cube);cabin.name="CarCabin";cabin.transform.SetParent(transform,false);cabin.transform.localScale=new Vector3(1.35f,.58f,1.65f);cabin.transform.localPosition=new Vector3(0,1.12f,-.25f);cabin.GetComponent<Renderer>().material=new Material(Shader.Find("Universal Render Pipeline/Lit")??Shader.Find("Standard")){color=new Color(.12f,.19f,.23f)};
  foreach(float z in new[]{-1.22f,1.22f})foreach(float x in new[]{-.96f,.96f}){var wheel=GameObject.CreatePrimitive(PrimitiveType.Cylinder);wheel.name="Wheel";wheel.transform.SetParent(transform,false);wheel.transform.localPosition=new Vector3(x,.35f,z);wheel.transform.localRotation=Quaternion.Euler(0,0,90);wheel.transform.localScale=new Vector3(.55f,.18f,.55f);wheel.GetComponent<Renderer>().material=new Material(Shader.Find("Universal Render Pipeline/Lit")??Shader.Find("Standard")){color=new Color(.045f,.045f,.05f)};}
 }
 public void SetControlled(bool v){Controlled=v;if(!v&&rb)rb.linearVelocity=Vector3.zero;}
 public void ToggleEngine(){EngineOn=!EngineOn;if(!EngineOn&&rb)rb.linearVelocity=Vector3.zero;}
 public void ToggleLights(){LightsOn=!LightsOn;}
 public void ToggleHazards(){HazardsOn=!HazardsOn;}
 public void ToggleSeatbelt(){SeatbeltOn=!SeatbeltOn;}
 public void ToggleWipers(){WipersOn=!WipersOn;}
 public void ToggleDoor(){FrontLeftDoorOpen=!FrontLeftDoorOpen;}
 public void Drive(Vector2 i){if(!Controlled||!EngineOn||rb==null)return;float t=i.y;rb.AddForce(transform.forward*t*acceleration,ForceMode.Acceleration);var l=transform.InverseTransformDirection(rb.linearVelocity);l.z=Mathf.Clamp(l.z,-maxSpeed,maxSpeed);rb.linearVelocity=transform.TransformDirection(l);transform.Rotate(0,i.x*turnSpeed*Time.deltaTime*Mathf.Clamp01(rb.linearVelocity.magnitude/3),0);}
}}
