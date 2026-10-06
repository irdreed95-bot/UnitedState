using UnityEngine;
using UnityEngine.EventSystems;

namespace CorruptStateRP {
public sealed class MobileActionHUD:MonoBehaviour {
 TouchControls controls; PlayerController player; VehicleController vehicle;
 public void Bind(TouchControls c,PlayerController p,VehicleController v){controls=c;player=p;vehicle=v;Build();}
 void Build(){
  var canvas=new GameObject("MobileActionCanvas").AddComponent<Canvas>();canvas.renderMode=RenderMode.ScreenSpaceOverlay;canvas.sortingOrder=150;
  canvas.gameObject.AddComponent<UnityEngine.UI.CanvasScaler>();canvas.gameObject.AddComponent<UnityEngine.UI.GraphicRaycaster>();
  Add(canvas.transform,"قفز",()=>player?.Jump(),new Vector2(-210,80),new Vector2(120,60));
  Add(canvas.transform,"سلاح",()=>player?.ToggleWeapon(),new Vector2(-350,80),new Vector2(120,60));
  Add(canvas.transform,"تصويب",()=>player?.ToggleAim(),new Vector2(-350,150),new Vector2(120,60));
  Add(canvas.transform,"إطلاق",()=>player?.Fire(),new Vector2(-210,150),new Vector2(120,60));
  Add(canvas.transform,"تغطية",()=>player?.ToggleCover(),new Vector2(-350,220),new Vector2(120,60));
  Add(canvas.transform,"نوم",()=>player?.ToggleSleep(),new Vector2(-210,220),new Vector2(120,60));
 }
 void Add(Transform p,string label,UnityEngine.Events.UnityAction action,Vector2 pos,Vector2 size){
  var g=new GameObject(label);g.transform.SetParent(p,false);var r=g.AddComponent<RectTransform>();r.anchorMin=r.anchorMax=new Vector2(1,0);r.pivot=new Vector2(1,0);r.anchoredPosition=pos;r.sizeDelta=size;
  var im=g.AddComponent<UnityEngine.UI.Image>();im.color=new Color(.08f,.1f,.13f,.88f);var b=g.AddComponent<UnityEngine.UI.Button>();b.targetGraphic=im;b.onClick.AddListener(action);
  var tx=new GameObject("Text").AddComponent<UnityEngine.UI.Text>();tx.transform.SetParent(g.transform,false);tx.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");tx.text=label;tx.fontSize=16;tx.alignment=TextAnchor.MiddleCenter;tx.color=Color.white;tx.rectTransform.anchorMin=Vector2.zero;tx.rectTransform.anchorMax=Vector2.one;tx.rectTransform.offsetMin=tx.rectTransform.offsetMax=Vector2.zero;
 }
}}