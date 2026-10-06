using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using CorruptStateRP.UI;
using CorruptStateRP.Citizens;

namespace CorruptStateRP.Lobby
{
    public sealed class LobbyScreen : MonoBehaviour
    {
        Font font;
        RectTransform content;
        NewCitizenProgram citizenProgram;
        GameObject shopPanel;

        void Awake()
        {
            font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            var cp = new GameObject("NewCitizenProgram");
            citizenProgram = cp.AddComponent<NewCitizenProgram>();
            Build();
        }

        void Build()
        {
            var canvas = new GameObject("LobbyCanvas").AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.gameObject.AddComponent<CanvasScaler>();
            canvas.gameObject.AddComponent<GraphicRaycaster>();
            canvas.gameObject.AddComponent<ResponsiveCanvas>();

            var safe = new GameObject("SafeArea", typeof(RectTransform), typeof(SafeAreaPanel));
            safe.transform.SetParent(canvas.transform, false);
            var safeRt = safe.GetComponent<RectTransform>();
            safeRt.anchorMin = Vector2.zero; safeRt.anchorMax = Vector2.one;
            safeRt.offsetMin = safeRt.offsetMax = Vector2.zero;
            safe.AddComponent<Image>().color = new Color(.018f,.02f,.027f,1);

            content = Panel(safe.transform, new Color(.025f,.03f,.04f,.96f));
            content.anchorMin = new Vector2(.035f,.045f);
            content.anchorMax = new Vector2(.965f,.955f);
            content.offsetMin = content.offsetMax = Vector2.zero;

            Text(content, "CORRUPT STATE RP", 30, TextAnchor.UpperLeft, new Vector2(28,-24), new Vector2(520,48));
            Text(content, "اللوبي", 28, TextAnchor.UpperRight, new Vector2(-28,-24), new Vector2(260,48));

            var profile = Panel(content, new Color(.07f,.075f,.09f,1));
            Anchor(profile,.035f,.67f,.965f,.94f);
            Text(profile, "المواطن الجديد", 26, TextAnchor.UpperRight, new Vector2(-24,-18), new Vector2(360,42));
            Text(profile, "Level 1", 22, TextAnchor.UpperRight, new Vector2(-24,-62), new Vector2(220,36));
            Text(profile, "XP 0 / 1000", 20, TextAnchor.UpperRight, new Vector2(-24,-100), new Vector2(250,34));
            Text(profile, "الرصيد  $1,000", 22, TextAnchor.UpperLeft, new Vector2(24,-62), new Vector2(300,36));
            Text(profile, "الحالة: New Citizen", 18, TextAnchor.UpperLeft, new Vector2(24,-102), new Vector2(330,34));

            var missions = Panel(content, new Color(.055f,.06f,.075f,1));
            Anchor(missions,.035f,.28f,.635f,.64f);
            Text(missions, "المهام اليومية", 24, TextAnchor.UpperRight, new Vector2(-20,-16), new Vector2(300,40));
            Mission(missions, "إكمال مهمة مواطن", "+$500  •  +100 XP", -64);
            Mission(missions, "زيارة البنك", "+$750  •  +150 XP", -126);
            Mission(missions, "التعرف على 5 مواطنين", "+$1,000  •  +250 XP", -188);

            var factions = Panel(content, new Color(.055f,.06f,.075f,1));
            Anchor(factions,.665f,.28f,.965f,.64f);
            Text(factions, "الفصائل", 24, TextAnchor.UpperRight, new Vector2(-20,-16), new Vector2(250,40));
            Text(factions, "الشرطة  •  الجيش  •  الصحة\nالدفاع المدني  •  القضاء\nالحكومة  •  الأمن الوطني  •  العصابات", 18, TextAnchor.UpperRight, new Vector2(-20,-70), new Vector2(330,150));

            Button(content, "المتجر والعروض", () => ToggleShop(), new Vector2(.035f,.12f), new Vector2(.28f,.23f));
            Button(content, "برنامج المواطنين", () => citizenProgram.Show(), new Vector2(.30f,.12f), new Vector2(.545f,.23f));
            Button(content, "دخول المدينة", () => SceneManager.LoadScene("CityRPScene"), new Vector2(.565f,.12f), new Vector2(.965f,.23f));
        }


        void ToggleShop()
        {
            if (shopPanel == null)
            {
                shopPanel = new GameObject("ShopPanel");
                shopPanel.transform.SetParent(content,false);
                var rt=shopPanel.AddComponent<RectTransform>(); Anchor(rt,.10f,.20f,.90f,.80f);
                shopPanel.AddComponent<Image>().color=new Color(.025f,.03f,.04f,.99f);
                Text(shopPanel.transform,"المتجر والعروض",28,TextAnchor.UpperRight,new Vector2(-24,-24),new Vector2(-40,50));
                Text(shopPanel.transform,"مركبات بداية\nملابس وشخصيات\nعروض السكن\nباقات RP مستقبلية\n\nالمتجر متصل لاحقاً بنظام الاقتصاد والمخزون.",20,TextAnchor.UpperRight,new Vector2(-30,-95),new Vector2(-50,300));
                Button(shopPanel.transform,"إغلاق",()=>shopPanel.SetActive(false),new Vector2(.68f,.05f),new Vector2(.94f,.14f));
            }
            shopPanel.SetActive(true);
        }

        void Mission(Transform parent, string title, string reward, float y)
        {
            Text(parent, title, 18, TextAnchor.UpperRight, new Vector2(-20,y), new Vector2(-30,32));
            Text(parent, reward, 15, TextAnchor.UpperLeft, new Vector2(20,y), new Vector2(230,32));
        }

        RectTransform Panel(Transform parent, Color color)
        {
            var go = new GameObject("Panel");
            go.transform.SetParent(parent,false);
            var rt = go.AddComponent<RectTransform>();
            go.AddComponent<Image>().color = color;
            return rt;
        }

        void Anchor(RectTransform rt, float x1, float y1, float x2, float y2)
        {
            rt.anchorMin = new Vector2(x1,y1); rt.anchorMax = new Vector2(x2,y2);
            rt.offsetMin = rt.offsetMax = Vector2.zero;
        }

        Text Text(Transform parent,string value,int size,TextAnchor alignment,Vector2 pos,Vector2 sizeDelta)
        {
            var go = new GameObject("Text");
            go.transform.SetParent(parent,false);
            var t = go.AddComponent<Text>();
            t.font=font; t.text=value; t.fontSize=size; t.alignment=alignment; t.color=Color.white;
            t.horizontalOverflow=HorizontalWrapMode.Wrap; t.verticalOverflow=VerticalWrapMode.Overflow;
            var rt=t.rectTransform; rt.anchorMin=rt.anchorMax=new Vector2(.5f,.5f);
            rt.anchoredPosition=pos; rt.sizeDelta=sizeDelta;
            return t;
        }

        void Button(Transform parent,string value,UnityEngine.Events.UnityAction action,Vector2 min,Vector2 max)
        {
            var go=new GameObject(value);
            go.transform.SetParent(parent,false);
            var rt=go.AddComponent<RectTransform>();
            rt.anchorMin=min; rt.anchorMax=max; rt.offsetMin=rt.offsetMax=Vector2.zero;
            var image=go.AddComponent<Image>(); image.color=new Color(.55f,.045f,.06f,1);
            var b=go.AddComponent<Button>(); b.targetGraphic=image; b.onClick.AddListener(action);
            Text(go.transform,value,19,TextAnchor.MiddleCenter,Vector2.zero,new Vector2(-16,-10));
        }
    }
}