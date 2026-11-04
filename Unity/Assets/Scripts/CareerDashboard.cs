using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace CorruptStateRP {
public sealed class CareerDashboard:MonoBehaviour {
 JobAndFactionSystem careers;Canvas canvas;GameObject panel;Text status;
 public void Bind(JobAndFactionSystem source){careers=source;Build();}
 public void Show(){if(panel!=null)panel.SetActive(true);}
 public void Hide(){if(panel!=null)panel.SetActive(false);}
 void Build(){
  if(careers==null||panel!=null)return;
  canvas=new GameObject("CareerDashboardCanvas").AddComponent<Canvas>();canvas.renderMode=RenderMode.ScreenSpaceOverlay;canvas.sortingOrder=230;
  var scaler=canvas.gameObject.AddComponent<CanvasScaler>();scaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;scaler.referenceResolution=new Vector2(1920,1080);scaler.matchWidthOrHeight=.5f;canvas.gameObject.AddComponent<GraphicRaycaster>();
  panel=new GameObject("CareerPanel");panel.transform.SetParent(canvas.transform,false);var pr=panel.AddComponent<RectTransform>();pr.anchorMin=new Vector2(.04f,.04f);pr.anchorMax=new Vector2(.96f,.96f);pr.offsetMin=pr.offsetMax=Vector2.zero;panel.AddComponent<Image>().color=new Color(.025f,.03f,.04f,.98f);
  Label(panel.transform,"المهن والفصائل",30,TextAnchor.MiddleCenter,new Vector2(0,-25),new Vector2(800,60));
  status=Label(panel.transform,"اختر مسارك في المدينة",18,TextAnchor.MiddleCenter,new Vector2(0,-75),new Vector2(800,42));
  Label(panel.transform,"الوظائف المتاحة",22,TextAnchor.MiddleCenter,new Vector2(-420,-125),new Vector2(700,42));
  Label(panel.transform,"الفصائل الرسمية",22,TextAnchor.MiddleCenter,new Vector2(420,-125),new Vector2(700,42));
  var jobs=new List<KeyValuePair<string,int>>(careers.Salaries);
  BuildJobScroll(panel.transform,jobs,new Vector2(.035f,.12f),new Vector2(.485f,.78f));
  BuildFactionScroll(panel.transform,careers.FactionNames,new Vector2(.515f,.12f),new Vector2(.965f,.78f));
  ButtonAt(panel.transform,"إغلاق",Hide,new Vector2(.72f,.025f),new Vector2(.96f,.025f),new Vector2(0,20));
  panel.SetActive(false);
 }
 void BuildJobScroll(Transform parent,IReadOnlyList<KeyValuePair<string,int>> jobs,Vector2 min,Vector2 max){
  var scroll=CreateScroll(parent,"JobsScroll",min,max,out var content);
  foreach(var item in jobs){string job=item.Key;int salary=item.Value;AddOption(content,job+" • $"+salary,()=>ChooseJob(job));}
 }
 void BuildFactionScroll(Transform parent,IReadOnlyList<string> factions,Vector2 min,Vector2 max){
  var scroll=CreateScroll(parent,"FactionsScroll",min,max,out var content);
  foreach(var faction in factions){string selected=faction;AddOption(content,selected,()=>JoinFaction(selected));}
 }
 ScrollRect CreateScroll(Transform parent,string name,Vector2 min,Vector2 max,out RectTransform content){
  var root=new GameObject(name);root.transform.SetParent(parent,false);var rr=root.AddComponent<RectTransform>();rr.anchorMin=min;rr.anchorMax=max;rr.offsetMin=rr.offsetMax=Vector2.zero;
  var scroll=root.AddComponent<ScrollRect>();scroll.horizontal=false;scroll.vertical=true;scroll.movementType=ScrollRect.MovementType.Clamped;
  var viewport=new GameObject("Viewport");viewport.transform.SetParent(root.transform,false);var vr=viewport.AddComponent<RectTransform>();vr.anchorMin=Vector2.zero;vr.anchorMax=Vector2.one;vr.offsetMin=vr.offsetMax=Vector2.zero;viewport.AddComponent<Image>().color=new Color(1,1,1,.015f);viewport.AddComponent<Mask>().showMaskGraphic=false;
  var body=new GameObject("Content");body.transform.SetParent(viewport.transform,false);content=body.AddComponent<RectTransform>();content.anchorMin=new Vector2(0,1);content.anchorMax=new Vector2(1,1);content.pivot=new Vector2(.5f,1);content.anchoredPosition=Vector2.zero;content.sizeDelta=new Vector2(0,0);
  var layout=body.AddComponent<VerticalLayoutGroup>();layout.spacing=8;layout.padding=new RectOffset(8,8,8,8);layout.childControlWidth=true;layout.childControlHeight=false;layout.childForceExpandWidth=true;layout.childForceExpandHeight=false;
  var fitter=body.AddComponent<ContentSizeFitter>();fitter.verticalFit=ContentSizeFitter.FitMode.PreferredSize;
  scroll.viewport=vr;scroll.content=content;return scroll;
 }
 void AddOption(Transform parent,string value,UnityEngine.Events.UnityAction action){
  var go=new GameObject("CareerOption");go.transform.SetParent(parent,false);var rt=go.AddComponent<RectTransform>();rt.sizeDelta=new Vector2(0,50);var image=go.AddComponent<Image>();image.color=new Color(.48f,.045f,.06f,.96f);var button=go.AddComponent<Button>();button.targetGraphic=image;button.onClick.AddListener(action);Label(go.transform,value,17,TextAnchor.MiddleCenter,Vector2.zero,new Vector2(-12,-8));
 }
 void ChooseJob(string job){if(careers!=null&&careers.ChooseJob(job))status.text="تم اختيار وظيفة: "+job+" • الراتب الأساسي $"+careers.Salary;else status.text="تعذر اختيار الوظيفة";}
 void JoinFaction(string faction){if(careers!=null&&careers.JoinFaction(faction))status.text="تم تقديم اختيارك للفصيل: "+faction;else status.text="هذا الفصيل غير متاح";}
 Text Label(Transform parent,string value,int size,TextAnchor align,Vector2 pos,Vector2 dimensions){var go=new GameObject("Text");go.transform.SetParent(parent,false);var t=go.AddComponent<Text>();t.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");t.text=UI.ArabicFixer.Fix(value);t.fontSize=size;t.alignment=align;t.color=Color.white;t.horizontalOverflow=HorizontalWrapMode.Wrap;t.verticalOverflow=VerticalWrapMode.Overflow;t.rectTransform.anchoredPosition=pos;t.rectTransform.sizeDelta=dimensions;return t;}
 void ButtonAt(Transform parent,string value,UnityEngine.Events.UnityAction action,Vector2 min,Vector2 max,Vector2 pos){var go=new GameObject("Career_"+value);go.transform.SetParent(parent,false);var rt=go.AddComponent<RectTransform>();rt.anchorMin=min;rt.anchorMax=max;rt.anchoredPosition=pos;rt.sizeDelta=new Vector2(0,46);var image=go.AddComponent<Image>();image.color=new Color(.48f,.045f,.06f,.96f);var button=go.AddComponent<Button>();button.targetGraphic=image;button.onClick.AddListener(action);Label(go.transform,value,17,TextAnchor.MiddleCenter,Vector2.zero,new Vector2(-12,-8));}
}}
