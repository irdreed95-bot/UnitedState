using UnityEngine;
using UnityEngine.UI;

namespace CorruptStateRP.UI {
    [RequireComponent(typeof(CanvasScaler))]
    public sealed class ResponsiveCanvas : MonoBehaviour {
        [SerializeField] Vector2 referenceResolution=new Vector2(1920f,1080f);
        [Range(0f,1f)][SerializeField] float matchWidthOrHeight=0.5f;
        void Awake(){
            CanvasScaler scaler=GetComponent<CanvasScaler>();
            scaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution=referenceResolution;
            scaler.screenMatchMode=CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight=matchWidthOrHeight;
            scaler.referencePixelsPerUnit=100f;
        }
    }
}
