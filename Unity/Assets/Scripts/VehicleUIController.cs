using UnityEngine;
namespace CorruptStateRP {
 public sealed class VehicleUIController:MonoBehaviour {
  [SerializeField] VehicleController vehicle;
  public void Bind(VehicleController target){vehicle=target;}
  public void ToggleDoor(){Debug.Log("Vehicle door toggle");}
  public void ToggleEngine(){if(vehicle)vehicle.ToggleEngine();}
  public void Accelerate(bool pressed){Debug.Log("Accelerator: "+pressed);}
  public void Brake(bool pressed){Debug.Log("Brake: "+pressed);}
  public void Steer(float value){Debug.Log("Steer: "+Mathf.Clamp(value,-1f,1f));}
  public void ToggleLights(){Debug.Log("Vehicle lights toggle");}
  public void ToggleHazards(){Debug.Log("Vehicle indicators/hazards toggle");}
  public void Horn(){Debug.Log("Vehicle horn");}
  public void ToggleSeatbelt(){Debug.Log("Vehicle seatbelt toggle");}
  public void ToggleWipers(){Debug.Log("Vehicle wipers toggle");}
 }
}