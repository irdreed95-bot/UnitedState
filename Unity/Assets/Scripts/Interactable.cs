using UnityEngine;
namespace CorruptStateRP {
public sealed class Interactable:MonoBehaviour{
 public string interactionName="تفاعل";[TextArea]public string description="";public bool CanInteract=>enabled;
 public string Interact(GameObject actor){return description;}
}
