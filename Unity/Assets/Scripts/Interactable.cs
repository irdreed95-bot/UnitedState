using UnityEngine;

namespace CorruptStateRP
{
    public sealed class Interactable : MonoBehaviour
    {
        [SerializeField] private string interactionName = "تفاعل";
        [TextArea]
        [SerializeField] private string description = "";

        public string InteractionName => interactionName;
        public string Description => description;
        public bool CanInteract => enabled && gameObject.activeInHierarchy;

        public string Interact(GameObject actor)
        {
            return description;
        }
    }
}