using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CorruptStateRP {
public sealed class CityInteractionSystem : MonoBehaviour {
    readonly string[] places={"مركز الشرطة","المستشفى","البنك المركزي","دار الحكومة","المحكمة"};
    readonly HashSet<string> visited=new();
    PlayerController player; Text prompt; Text progress; Canvas canvas;

    void Start(){ player=FindFirstObjectByType<PlayerController>(); BuildUI(); CreateInteractionPoints(); }
    void Update(){
        if(player==null) player=FindFirstObjectByType<PlayerController>();
        if(player==null)return;
        string nearest=null; float best=5f;
        foreach(var n in places){var go=GameObject.Find(n);if(!go)continue;float d=Vector3.Distance(player.transform.position,go.transform.position);if(d<best){best=d;nearest=n;}}
        prompt.text=nearest==null?"اقترب من مؤسسة للتفاعل":$"اضغط E للتفاعل • {nearest}";
        if(nearest!=null && Input.GetKeyDown(KeyCode.E)) Interact(nearest);
    }
    void CreateInteractionPoints(){
        foreach(var n in places){var go=GameObject.Find(n);if(go!=null){var i=go.GetComponent<Interactable>()??go.AddComponent<Interactable>();}}
    }
    void Interact(string place){visited.Add(place); progress.text=$"المؤسسات: {visited.Count}/5\nآخر زيارة: {place}"; prompt.text=$"✓ تم تسجيل زيارتك: {place}"; }
    void BuildUI(){
        canvas=new GameObject("CityInteractionHUD").AddComponent<Canvas>();canvas.renderMode=RenderMode.ScreenSpaceOverlay;canvas.sortingOrder=120;
        var scaler=canvas.gameObject.AddComponent<CanvasScaler>();scaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;scaler.referenceResolution=new Vector2(1920,1080);
        canvas.gameObject.AddComponent<GraphicRaycaster>();
        prompt=Text("InteractionPrompt",new Vector2(0,-420),new Vector2(800,60),24,TextAnchor.MiddleCenter);
        progress=Text("InstitutionProgress",new Vector2(-760,-80),new Vector2(300,80),18,TextAnchor.UpperLeft);progress.text="المؤسسات: 0/5";
    }
    Text Text(string n,Vector2 pos,Vector2 size,int fs,TextAnchor a){
        var go=new GameObject(n);go.transform.SetParent(canvas.transform,false);var t=go.AddComponent<Text>();t.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");t.fontSize=fs;t.color=Color.white;t.alignment=a;t.rectTransform.anchoredPosition=pos;t.rectTransform.sizeDelta=size;return t;
    }
}
}