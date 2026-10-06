using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CorruptStateRP {
public sealed class FactionDashboard : MonoBehaviour {
    readonly string[] factions={"الشرطة","الجيش","الصحة والإسعاف","الدفاع المدني","القضاء","الحكومة","الأمن الوطني","العصابات"};
    readonly Dictionary<string,int> members=new();
    Canvas canvas; GameObject panel; Text status;

    void Awake(){foreach(var f in factions)members[f]=0; Build();}

    public void Show(){panel.SetActive(true);}
    public void Hide(){panel.SetActive(false);}
    public void Join(string faction){if(!members.ContainsKey(faction))return;members[faction]++;status.text=$"تم تسجيلك ضمن: {faction}";}

    void Build(){
        canvas=new GameObject("FactionDashboardCanvas").AddComponent<Canvas>();
        canvas.renderMode=RenderMode.ScreenSpaceOverlay;canvas.sortingOrder=180;
        canvas.gameObject.AddComponent<CanvasScaler>();canvas.gameObject.AddComponent<GraphicRaycaster>();
        panel=new GameObject("FactionPanel");panel.transform.SetParent(canvas.transform,false);
        var rt=panel.AddComponent<RectTransform>();rt.anchorMin=new Vector2(.08f,.08f);rt.anchorMax=new Vector2(.92f,.92f);rt.offsetMin=rt.offsetMax=Vector2.zero;
        panel.AddComponent<Image>().color=new Color(.025f,.03f,.04f,.98f);
        Text(panel.transform,"الفصائل والوظائف الرسمية",30,TextAnchor.UpperCenter,new Vector2(0,-28),new Vector2(700,55));
        status=Text(panel.transform,"اختر مسارك داخل المدينة",18,TextAnchor.UpperCenter,new Vector2(0,-78),new Vector2(700,40));
        float y=-135;
        for(int i=0;i<factions.Length;i++){string f=factions[i];Button(panel.transform,f,()=>Join(f),new Vector2(.08f,0),new Vector2(.46f,0),y);y-=65;if(i==3)y=-135;}
        y=-135;
        for(int i=4;i<factions.Length;i++){string f=factions[i];Button(panel.transform,f,()=>Join(f),new Vector2(.54f,0),new Vector2(.92f,0),y);y-=65;}
        Button(panel.transform,"إغلاق",Hide,new Vector2(.70f,0),new Vector2(.92f,0),-390);
        panel.SetActive(false);
    }
    Text Text(Transform p,string v,int fs,TextAnchor a,Vector2 pos,Vector2 size){var g=new GameObject("Text");g.transform.SetParent(p,false);var t=g.AddComponent<Text>();t.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");t.text=v;t.fontSize=fs;t.alignment=a;t.color=Color.white;t.rectTransform.anchoredPosition=pos;t.rectTransform.sizeDelta=size;return t;}
    void Button(Transform p,string v,UnityEngine.Events.UnityAction action,Vector2 min,Vector2 max,float y){var g=new GameObject(v);g.transform.SetParent(p,false);var rt=g.AddComponent<RectTransform>();rt.anchorMin=new Vector2(min.x,.5f);rt.anchorMax=new Vector2(max.x,.5f);rt.anchoredPosition=new Vector2(0,y);rt.sizeDelta=new Vector2(0,50);var im=g.AddComponent<Image>();im.color=new Color(.55f,.045f,.06f,1);var b=g.AddComponent<Button>();b.targetGraphic=im;b.onClick.AddListener(action);Text(g.transform,v,17,TextAnchor.MiddleCenter,Vector2.zero,new Vector2(-10,-8));}
}
}