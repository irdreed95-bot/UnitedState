using UnityEngine;
namespace CorruptStateRP {
public sealed class VehicleUIController:MonoBehaviour {
 [SerializeField] VehicleController vehicle;
 public void Bind(VehicleController target){vehicle=target;}
 public void ToggleDoor(){if(vehicle)vehicle.ToggleDoor();}
 public void ToggleEngine(){if(vehicle)vehicle.ToggleEngine();}
 public void Accelerate(bool pressed){Debug.Log("Accelerator: "+pressed);}
 public void Brake(bool pressed){Debug.Log("Brake: "+pressed);}
 public void Steer(float value){Debug.Log("Steer: "+Mathf.Clamp(value,-1f,1f));}
 public void ToggleLights(){if(vehicle)vehicle.ToggleLights();}
 public void ToggleHazards(){if(vehicle)vehicle.ToggleHazards();}
 public void Horn(){Debug.Log("Vehicle horn");}
 public void ToggleSeatbelt(){if(vehicle)vehicle.ToggleSeatbelt();}
 public void ToggleWipers(){if(vehicle)vehicle.ToggleWipers();}
}}