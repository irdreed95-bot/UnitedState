using System;
using UnityEngine;
using UnityEngine.UI;
using CorruptStateRP.Auth;

namespace CorruptStateRP.Citizens
{
    public sealed class NewCitizenProgram : MonoBehaviour
    {
        [Serializable]
        public sealed class Mission
        {
            public string title;
            public string requirement;
            public string reward;
            public bool completed;
        }

        readonly Mission[] missions =
        {
            new Mission { title="الهوية الوطنية", requirement="إكمال إجراءات الهوية الوطنية والبلدية.", reward="$1,000 + بطاقة هوية" },
            new Mission { title="الحساب البنكي", requirement="فتح حساب بنكي والحصول على بطاقة واستخدام الصراف.", reward="بطاقة بنكية + $500" },
            new Mission { title="رخصة القيادة", requirement="اجتياز الاختبار النظري والعملي للقيادة.", reward="رخصة قيادة + 500 XP" },
            new Mission { title="أول مركبة", requirement="شراء وتسجيل أول مركبة.", reward="خصم 10% على الصيانة" },
            new Mission { title="أول وظيفة", requirement="اختيار أول وظيفة وتحقيق دخل قانوني بقيمة $5,000.", reward="$2,000 + لقب المواطن المجتهد" },
            new Mission { title="التعرف على المؤسسات", requirement="زيارة البلدية والشرطة والمستشفى والبنك والمحكمة.", reward="$1,500 + 1,000 XP" },
            new Mission { title="المواطن الملتزم", requirement="30 دقيقة بدون مخالفات أو بلاغات أو جرائم.", reward="شهادة المواطن الملتزم + $2,000" },
            new Mission { title="التعرف على المدينة", requirement="زيارة المطار والميناء والحديقة والجامعة والمنطقة التجارية.", reward="خريطة المدينة + $1,000" },
            new Mission { title="التفاعل مع المجتمع", requirement="إلقاء التحية على 5 لاعبين وإجراء أول معاملة RP.", reward="$1,500 + وسام المواطن الاجتماعي" },
            new Mission { title="اختيار المستقبل", requirement="إكمال برنامج المواطنين الجدد واختيار المسار المهني.", reward="$10,000 + سيارة بداية + منزل 7 أيام + فتح الوظائف الرسمية" }
        };

        Canvas canvas;
        Text progressText;
        RectTransform listRoot;
        RectTransform rootPanel;
        int selectedMission;
        AuthClient auth;

        public Mission[] Missions => missions;

        void Awake()
        {
            DontDestroyOnLoad(gameObject);
            auth = FindFirstObjectByType<AuthClient>();
            BuildUI();
        }

        void CompleteSelectedMission()
        {
            if (selectedMission < 0 || selectedMission >= missions.Length) return;
            auth = FindFirstObjectByType<AuthClient>();
            if (auth == null || !auth.HasSession)
            {
                Debug.LogWarning("Mission completion requires an authenticated server session.");
                return;
            }
            StartCoroutine(auth.CompleteMission(selectedMission + 1, (json, error) =>
            {
                if (error != null) { Debug.LogWarning("Server rejected mission completion: " + error); return; }
                missions[selectedMission].completed = true;
                Refresh();
            }));
        }

        void BuildUI()
        {
            canvas = FindFirstObjectByType<Canvas>();
            if (canvas == null)
            {
                var go = new GameObject("CitizenProgramCanvas");
                canvas = go.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                go.AddComponent<CanvasScaler>();
                go.AddComponent<GraphicRaycaster>();
            }

            rootPanel = Panel(canvas.transform, new Color(.025f,.03f,.04f,.96f));
            var root = rootPanel;
            root.anchorMin = new Vector2(.53f,.06f);
            root.anchorMax = new Vector2(.97f,.94f);
            root.offsetMin = root.offsetMax = Vector2.zero;

            Text(root, "برنامج المواطنين الجدد", 34, TextAnchor.MiddleCenter, new Vector2(0,-24), new Vector2(-30,64));
            progressText = Text(root, "", 19, TextAnchor.MiddleCenter, new Vector2(0,-78), new Vector2(-30,42));

            var scroll = new GameObject("MissionList");
            scroll.transform.SetParent(root,false);
            var sr = scroll.AddComponent<RectTransform>();
            sr.anchorMin = new Vector2(.04f,.10f);
            sr.anchorMax = new Vector2(.96f,.82f);
            sr.offsetMin = sr.offsetMax = Vector2.zero;
            var layout = scroll.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 10;
            layout.childControlWidth = true;
            layout.childControlHeight = false;
            listRoot = sr;

            var complete = Button(root, "إرسال إكمال المهمة إلى الخادم", () => CompleteSelectedMission());
            complete.GetComponent<RectTransform>().anchorMin = new Vector2(.04f,.025f);
            complete.GetComponent<RectTransform>().anchorMax = new Vector2(.48f,.085f);
            complete.GetComponent<RectTransform>().offsetMin = complete.GetComponent<RectTransform>().offsetMax = Vector2.zero;

            var close = Button(root, "إغلاق", () => root.gameObject.SetActive(false));
            close.GetComponent<RectTransform>().anchorMin = new Vector2(.68f,.025f);
            close.GetComponent<RectTransform>().anchorMax = new Vector2(.96f,.085f);
            close.GetComponent<RectTransform>().offsetMin = close.GetComponent<RectTransform>().offsetMax = Vector2.zero;

            Refresh();
            rootPanel.gameObject.SetActive(false);
        }

        public void Show()
        {
            if (rootPanel != null) { rootPanel.gameObject.SetActive(true); Refresh(); }
        }

        void Refresh()
        {
            if (progressText != null)
                progressText.text = $"التقدم: {CompletedCount()}/{missions.Length}  •  الهدف النهائي: مواطن الجمهورية";

            if (listRoot == null) return;
            for (int i = listRoot.childCount - 1; i >= 0; i--) Destroy(listRoot.GetChild(i).gameObject);

            for (int i = 0; i < missions.Length; i++)
            {
                int index = i;
                var card = new GameObject("Mission_" + (i + 1));
                card.transform.SetParent(listRoot,false);
                var image = card.AddComponent<Image>();
                image.color = missions[i].completed
                    ? new Color(.08f,.25f,.14f,.95f)
                    : new Color(.07f,.08f,.11f,.95f);

                var rt = card.GetComponent<RectTransform>();
                rt.sizeDelta = new Vector2(0,118);

                var title = Text(card.transform,
                    (missions[i].completed ? "✓ " : "○ ") + (i + 1) + ". " + missions[i].title,
                    22, TextAnchor.MiddleRight, new Vector2(-12,-8), new Vector2(-24,38));

                var body = Text(card.transform,
                    missions[i].requirement + "\nالمكافأة: " + missions[i].reward,
                    16, TextAnchor.UpperRight, new Vector2(-12,-52), new Vector2(-24,62));

                title.horizontalOverflow = HorizontalWrapMode.Wrap;
                body.horizontalOverflow = HorizontalWrapMode.Wrap;
                card.AddComponent<Button>().onClick.AddListener(() => selectedMission = index);
            }
        }

        int CompletedCount()
        {
            int count = 0;
            foreach (var m in missions) if (m.completed) count++;
            return count;
        }

        RectTransform Panel(Transform parent, Color color)
        {
            var go = new GameObject("Panel");
            go.transform.SetParent(parent,false);
            var rt = go.AddComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = new Vector2(.5f,.5f);
            rt.sizeDelta = new Vector2(900,900);
            var image = go.AddComponent<Image>();
            image.color = color;
            return rt;
        }

        Text Text(Transform parent, string value, int size, TextAnchor anchor, Vector2 position, Vector2 sizeDelta)
        {
            var go = new GameObject("Text");
            go.transform.SetParent(parent,false);
            var t = go.AddComponent<Text>();
            t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            t.text = value;
            t.fontSize = size;
            t.alignment = anchor;
            t.color = Color.white;
            t.raycastTarget = false;
            var rt = t.rectTransform;
            rt.anchorMin = rt.anchorMax = new Vector2(.5f,.5f);
            rt.anchoredPosition = position;
            rt.sizeDelta = sizeDelta;
            return t;
        }

        Button Button(Transform parent, string value, UnityEngine.Events.UnityAction action)
        {
            var go = new GameObject(value);
            go.transform.SetParent(parent,false);
            var image = go.AddComponent<Image>();
            image.color = new Color(.55f,.05f,.06f,1);
            var button = go.AddComponent<Button>();
            button.targetGraphic = image;
            var text = Text(go.transform, value, 18, TextAnchor.MiddleCenter, Vector2.zero, new Vector2(-10,-10));
            text.raycastTarget = false;
            button.onClick.AddListener(action);
            return button;
        }
    }
}
