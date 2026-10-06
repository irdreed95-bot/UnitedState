using UnityEngine;
using UnityEngine.UI;

namespace CorruptStateRP {
public sealed class FactionLeaderUI : MonoBehaviour {
    Canvas canvas; GameObject panel; Text status;
    void Awake(){Build();}
    void Build(){
        canvas=new GameObject("FactionLeaderCanvas").AddComponent<Canvas>();
        canvas.renderMode=RenderMode.ScreenSpaceOverlay; canvas.sortingOrder=210;
        canvas.gameObject.AddComponent<CanvasScaler>(); canvas.gameObject.AddComponent<GraphicRaycaster>();

        var open=ButtonObj(canvas.transform,"قيادة الفصيل",()=>panel.SetActive(true));
        var ort=open.GetComponent<RectTransform>(); ort.anchorMin=ort.anchorMax=new Vector2(1,0); ort.pivot=new Vector2(1,0); ort.anchoredPosition=new Vector2(-24,24); ort.sizeDelta=new Vector2(170,48);

        panel=new GameObject("FactionLeaderPanel"); panel.transform.SetParent(canvas.transform,false);
        var pr=panel.AddComponent<RectTransform>(); pr.anchorMin=new Vector2(.1f,.1f); pr.anchorMax=new Vector2(.9f,.9f); pr.offsetMin=pr.offsetMax=Vector2.zero;
        panel.AddComponent<Image>().color=new Color(.025f,.03f,.04f,.99f);
        Label(panel.transform,"إدارة الفصيل",28,new Vector2(0,-28),new Vector2(700,55));
        status=Label(panel.transform,"صلاحيات القائد • اختر إجراءً",17,new Vector2(0,-70),new Vector2(700,40));
        string[] acts={"ترقية عضو","تنزيل رتبة","تعيين نائب","إنذار عضو","طرد عضو","منح إجازة","مكافأة عضو"};
        float y=-125;
        foreach(var a in acts){string x=a; var b=ButtonObj(panel.transform,x,()=>Do(x)); var br=b.GetComponent<RectTransform>(); br.anchorMin=new Vector2(.15f,.5f);br.anchorMax=new Vector2(.85f,.5f);br.anchoredPosition=new Vector2(0,y);br.sizeDelta=new Vector2(0,45);y-=52;}
        var close=ButtonObj(panel.transform,"إغلاق",()=>panel.SetActive(false));var cr=close.GetComponent<RectTransform>();cr.anchorMin=cr.anchorMax=new Vector2(.5f,0);cr.anchoredPosition=new Vector2(0,22);cr.sizeDelta=new Vector2(150,42);
        panel.SetActive(false);
    }
    void Do(string action){status.text="تم اختيار: "+action+" • بانتظار ربط اللاعب/السيرفر";Debug.Log("Faction leader action: "+action);}
    public void GrantLeave(int days){Debug.Log("Grant leave: "+days+" days");}
    public void DemoteMember(string playerId){Debug.Log("Demote: "+playerId);}
    public void IssueWarning(string playerId){Debug.Log("Warning: "+playerId);}
    public void KickMember(string playerId){Debug.Log("Kick: "+playerId);}
    public void PromoteMember(string playerId){Debug.Log("Promote: "+playerId);}
    public void AssignDeputy(string playerId){Debug.Log("Assign deputy: "+playerId);}
    public void RewardMember(string playerId,int amount,int xp){Debug.Log("Reward "+playerId+": $"+amount+", XP "+xp);}
    GameObject ButtonObj(Transform p,string v,UnityEngine.Events.UnityAction a){var g=new GameObject(v);g.transform.SetParent(p,false);var im=g.AddComponent<Image>();im.color=new Color(.13f,.15f,.19f,1);var b=g.AddComponent<Button>();b.targetGraphic=im;b.onClick.AddListener(a);Label(g.transform,v,15,Vector2.zero,new Vector2(0,40));return g;}
    Text Label(Transform p,string v,int fs,Vector2 pos,Vector2 size){var g=new GameObject("Label");g.transform.SetParent(p,false);var t=g.AddComponent<Text>();t.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");t.text=v;t.fontSize=fs;t.alignment=TextAnchor.MiddleCenter;t.color=Color.white;t.rectTransform.anchoredPosition=pos;t.rectTransform.sizeDelta=size;return t;}
}}