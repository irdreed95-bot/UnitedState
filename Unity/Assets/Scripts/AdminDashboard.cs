using UnityEngine;
using UnityEngine.UI;

namespace CorruptStateRP {
public sealed class AdminDashboard : MonoBehaviour {
    Canvas canvas; GameObject panel; Text status;
    void Awake(){Build();}
    void Build(){
        canvas=new GameObject("AdminDashboardCanvas").AddComponent<Canvas>();
        canvas.renderMode=RenderMode.ScreenSpaceOverlay;canvas.sortingOrder=220;
        canvas.gameObject.AddComponent<CanvasScaler>();canvas.gameObject.AddComponent<GraphicRaycaster>();
        var open=new GameObject("AdminButton");open.transform.SetParent(canvas.transform,false);
        var ort=open.AddComponent<RectTransform>();ort.anchorMin=new Vector2(1,1);ort.anchorMax=new Vector2(1,1);ort.pivot=new Vector2(1,1);ort.anchoredPosition=new Vector2(-24,-24);ort.sizeDelta=new Vector2(150,50);
        var oi=open.AddComponent<Image>();oi.color=new Color(.55f,.045f,.06f,1);
        var ob=open.AddComponent<Button>();ob.targetGraphic=oi;ob.onClick.AddListener(()=>panel.SetActive(true));Text(open.transform,"ADMIN",17,TextAnchor.MiddleCenter);
        panel=new GameObject("AdminPanel");panel.transform.SetParent(canvas.transform,false);
        var rt=panel.AddComponent<RectTransform>();rt.anchorMin=new Vector2(.08f,.08f);rt.anchorMax=new Vector2(.92f,.92f);rt.offsetMin=rt.offsetMax=Vector2.zero;
        panel.AddComponent<Image>().color=new Color(.02f,.025f,.035f,.99f);
        Text(panel.transform,"لوحة إدارة Corrupt State RP",30,TextAnchor.UpperCenter,new Vector2(0,-28),new Vector2(800,55));
        status=Text(panel.transform,"جاهز • أدوات الإدارة",18,TextAnchor.UpperCenter,new Vector2(0,-78),new Vector2(800,40));
        string[] actions={"بحث عن لاعب","سجن","فك السجن","كتم","طرد","حظر","انتقال للاعب","إحضار لاعب","قلب مركبة","Free Cam","المدراء المتصلون","سجل القتل","انتقال للعلامة"};
        float y=-140;
        foreach(var action in actions){string a=action;Button(panel.transform,a,()=>Action(a),new Vector2(.08f,.5f),new Vector2(.46f,.5f),y);y-=52;if(y<-350)y=-140;}
        y=-140;
        for(int i=7;i<actions.Length;i++){string a=actions[i];Button(panel.transform,a,()=>Action(a),new Vector2(.54f,.5f),new Vector2(.92f,.5f),y);y-=52;}
        Button(panel.transform,"إغلاق",()=>panel.SetActive(false),new Vector2(.70f,.15f),new Vector2(.92f,.15f),-395);
        panel.SetActive(false);
    }
    void Action(string action){status.text=$"تم اختيار أمر الإدارة: {action}";Debug.Log("Admin action: "+action);}
    Text Text(Transform p,string v,int fs,TextAnchor a){return Text(p,v,fs,a,Vector2.zero,new Vector2(300,45));}
    Text Text(Transform p,string v,int fs,TextAnchor a,Vector2 pos,Vector2 size){var g=new GameObject("Text");g.transform.SetParent(p,false);var t=g.AddComponent<Text>();t.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");t.text=v;t.fontSize=fs;t.alignment=a;t.color=Color.white;t.rectTransform.anchoredPosition=pos;t.rectTransform.sizeDelta=size;return t;}
    void Button(Transform p,string v,UnityEngine.Events.UnityAction action,Vector2 min,Vector2 max,float y){var g=new GameObject(v);g.transform.SetParent(p,false);var rt=g.AddComponent<RectTransform>();rt.anchorMin=new Vector2(min.x,.5f);rt.anchorMax=new Vector2(max.x,.5f);rt.anchoredPosition=new Vector2(0,y);rt.sizeDelta=new Vector2(0,42);var im=g.AddComponent<Image>();im.color=new Color(.12f,.14f,.18f,1);var b=g.AddComponent<Button>();b.targetGraphic=im;b.onClick.AddListener(action);Text(g.transform,v,15,TextAnchor.MiddleCenter,Vector2.zero,new Vector2(-8,-6));}
}
}