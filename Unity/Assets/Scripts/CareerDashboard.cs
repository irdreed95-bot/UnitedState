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
  panel=new GameObject("CareerPanel");panel.transform.SetParent(canvas.transform,false);var pr=panel.AddComponent<RectTransform>();pr.anchorMin=new Vector2(.08f,.08f);pr.anchorMax=new Vector2(.92f,.92f);pr.offsetMin=pr.offsetMax=Vector2.zero;panel.AddComponent<Image>().color=new Color(.025f,.03f,.04f,.98f);
  Label(panel.transform,"المهن والفصائل",30,TextAnchor.MiddleCenter,new Vector2(0,-28),new Vector2(800,60));
  status=Label(panel.transform,"اختر مسارك في المدينة",18,TextAnchor.MiddleCenter,new Vector2(0,-78),new Vector2(800,42));
  Label(panel.transform,"الوظائف المتاحة",22,TextAnchor.MiddleCenter,new Vector2(-390,-125),new Vector2(650,42));
  Label(panel.transform,"الفصائل الرسمية",22,TextAnchor.MiddleCenter,new Vector2(390,-125),new Vector2(650,42));
  float jobY=-175;int jobCount=0;
  foreach(var item in careers.Salaries){string job=item.Key;int salary=item.Value;ButtonAt(panel.transform,job+" • $"+salary,()=>ChooseJob(job),new Vector2(.04f,.5f),new Vector2(.48f,.5f),new Vector2(0,jobY));jobY-=56;jobCount++;if(jobCount>=8)break;}
  float factionY=-175;int factionCount=0;
  foreach(var faction in careers.FactionNames){string selected=faction;ButtonAt(panel.transform,selected,()=>JoinFaction(selected),new Vector2(.52f,.5f),new Vector2(.96f,.5f),new Vector2(0,factionY));factionY-=56;factionCount++;if(factionCount>=8)break;}
  ButtonAt(panel.transform,"إغلاق",Hide,new Vector2(.72f,.03f),new Vector2(.96f,.03f),new Vector2(0,24));
  panel.SetActive(false);
 }
 void ChooseJob(string job){if(careers!=null&&careers.ChooseJob(job))status.text="تم اختيار وظيفة: "+job+" • الراتب الأساسي $"+careers.Salary;else status.text="تعذر اختيار الوظيفة";}
 void JoinFaction(string faction){if(careers!=null&&careers.JoinFaction(faction))status.text="تم تقديم اختيارك للفصيل: "+faction;else status.text="هذا الفصيل غير متاح";}
 Text Label(Transform parent,string value,int size,TextAnchor align,Vector2 pos,Vector2 dimensions){var go=new GameObject("Text");go.transform.SetParent(parent,false);var t=go.AddComponent<Text>();t.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");t.text=UI.ArabicFixer.Fix(value);t.fontSize=size;t.alignment=align;t.color=Color.white;t.horizontalOverflow=HorizontalWrapMode.Wrap;t.verticalOverflow=VerticalWrapMode.Overflow;t.rectTransform.anchoredPosition=pos;t.rectTransform.sizeDelta=dimensions;return t;}
 void ButtonAt(Transform parent,string value,UnityEngine.Events.UnityAction action,Vector2 min,Vector2 max,Vector2 pos){var go=new GameObject("Career_"+value);go.transform.SetParent(parent,false);var rt=go.AddComponent<RectTransform>();rt.anchorMin=min;rt.anchorMax=max;rt.anchoredPosition=pos;rt.sizeDelta=new Vector2(0,46);var image=go.AddComponent<Image>();image.color=new Color(.48f,.045f,.06f,.96f);var button=go.AddComponent<Button>();button.targetGraphic=image;button.onClick.AddListener(action);Label(go.transform,value,17,TextAnchor.MiddleCenter,Vector2.zero,new Vector2(-12,-8));}
}}
