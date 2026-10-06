using UnityEngine;

namespace CorruptStateRP.UI {
    [RequireComponent(typeof(RectTransform))]
    public sealed class SafeAreaPanel : MonoBehaviour {
        RectTransform rect;
        Rect lastSafeArea;
        void Awake(){ rect=GetComponent<RectTransform>(); Apply(); }
        void OnEnable(){ Apply(); }
        void Update(){ if(lastSafeArea!=Screen.safeArea) Apply(); }
        void Apply(){
            if(rect==null) rect=GetComponent<RectTransform>();
            Rect safe=Screen.safeArea; lastSafeArea=safe;
            Vector2 min=safe.position; Vector2 max=safe.position+safe.size;
            min.x/=Screen.width; min.y/=Screen.height; max.x/=Screen.width; max.y/=Screen.height;
            rect.anchorMin=min; rect.anchorMax=max; rect.offsetMin=Vector2.zero; rect.offsetMax=Vector2.zero;
        }
    }
}
