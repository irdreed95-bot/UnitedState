using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using CorruptStateRP.Auth;
using CorruptStateRP.UI;

namespace CorruptStateRP.Onboarding {
    public sealed class OnboardingScreen : MonoBehaviour {
        public enum Mode { Splash, Login, Lobby }
        [SerializeField] Mode mode;
        const string DefaultApiUrl = "http://127.0.0.1:8081";
        Font font;
        InputField identifierField, passwordField, confirmField, endpointField;
        Text statusText, submitText, endpointHint, toggleText;
        Button submitButton, settingsButton, saveEndpointButton;
        GameObject confirmFieldObject, loginHintObject, endpointFieldObject, saveEndpointObject;
        bool registering, settingsOpen;

        void Awake() {
            font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (FindFirstObjectByType<EventSystem>() == null) {
                var e = new GameObject("EventSystem");
                e.AddComponent<EventSystem>();
                e.AddComponent<StandaloneInputModule>();
            }
        }

        public void Configure(Mode value) { mode = value; }

        void Start() { Build(); }

        void Build() {
            var canvasObject = new GameObject("Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster), typeof(ResponsiveCanvas));
            var canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = .5f;

            var safe = new GameObject("SafeArea", typeof(RectTransform), typeof(SafeAreaPanel));
            safe.transform.SetParent(canvas.transform, false);
            Stretch(safe.GetComponent<RectTransform>());

            var background = MakeImage(safe.transform, "CityArtwork", Vector2.zero, Vector2.one, new Color(.025f, .035f, .06f, 1));
            var artwork = Resources.Load<Sprite>("Art/LoginBackground");
            if (artwork != null) {
                background.sprite = artwork;
                background.preserveAspect = false;
            }
            MakeImage(safe.transform, "NightOverlay", Vector2.zero, Vector2.one, new Color(.012f, .018f, .032f, .38f));

            if (mode == Mode.Splash) {
                MakeText(safe.transform, "CORRUPT STATE RP", new Vector2(.12f, .38f), new Vector2(.88f, .66f), 76, new Color(1f, .78f, .28f), TextAnchor.MiddleCenter, true);
                MakeText(safe.transform, "لكل مدينة قصة... ولكل قصة بداية", new Vector2(.12f, .27f), new Vector2(.88f, .39f), 34, Color.white, TextAnchor.MiddleCenter, false);
                return;
            }
            if (mode != Mode.Login) return;

            BuildBrandPanel(safe.transform);
            BuildAuthPanel(safe.transform);
        }

        void BuildBrandPanel(Transform parent) {
            MakeText(parent, "CORRUPT\nSTATE RP", new Vector2(.055f, .64f), new Vector2(.49f, .91f), 76, new Color(1f, .78f, .28f), TextAnchor.MiddleLeft, true);
            MakeText(parent, "عالم مفتوح • حياة واقعية • قصتك تبدأ هنا", new Vector2(.06f, .53f), new Vector2(.48f, .63f), 30, Color.white, TextAnchor.MiddleLeft, true);
            MakeText(parent, "ادخل المدينة، اصنع هويتك، وابنِ مستقبلك بين الناس", new Vector2(.06f, .43f), new Vector2(.47f, .52f), 23, new Color(.83f, .86f, .91f), TextAnchor.MiddleLeft, false);

            string savedEndpoint = PlayerPrefs.GetString("CSRP_API_URL", "");
            bool configured = !string.IsNullOrWhiteSpace(savedEndpoint);
            MakeImage(parent, "ConnectionBadge", new Vector2(.065f, .29f), new Vector2(.42f, .37f), new Color(.015f, .025f, .035f, .82f));
            MakeText(parent, configured ? "●  رابط الخادم محفوظ" : "●  العنوان المحلي الافتراضي 8081", new Vector2(.08f, .29f), new Vector2(.41f, .37f), 22, configured ? new Color(.35f, 1f, .58f) : new Color(1f, .76f, .28f), TextAnchor.MiddleLeft, true);

            settingsButton = MakeButton(parent, "إعدادات الاتصال", new Vector2(.065f, .16f), new Vector2(.31f, .25f), 21, new Color(.07f, .085f, .11f, .9f), new Color(1f, .78f, .28f), ToggleEndpointSettings);
            endpointFieldObject = MakeInput(parent, "رابط API للخادم (https://...)", false, new Vector2(.065f, .16f), new Vector2(.42f, .25f), out endpointField);
            endpointField.text = string.IsNullOrWhiteSpace(savedEndpoint) ? DefaultApiUrl : savedEndpoint;
            endpointFieldObject.SetActive(false);
            saveEndpointObject = MakeButton(parent, "حفظ", new Vector2(.065f, .08f), new Vector2(.22f, .15f), 20, new Color(.68f, .43f, .08f, 1f), Color.black, SaveEndpoint).gameObject;
            saveEndpointObject.SetActive(false);
            endpointHint = MakeText(parent, "لا تشارك رابط الخادم الخاص مع الآخرين", new Vector2(.23f, .08f), new Vector2(.47f, .15f), 17, new Color(.9f, .9f, .9f), TextAnchor.MiddleLeft, false);
            endpointHint.gameObject.SetActive(false);
            MakeText(parent, "الإصدار التجريبي 0.1.0", new Vector2(.06f, .025f), new Vector2(.34f, .075f), 18, new Color(.82f, .84f, .88f), TextAnchor.MiddleLeft, false);
        }

        void BuildAuthPanel(Transform parent) {
            var card = MakeImage(parent, "AuthenticationCard", new Vector2(.55f, .105f), new Vector2(.95f, .91f), new Color(.012f, .018f, .03f, .91f));
            card.raycastTarget = false;
            MakeImage(card.transform, "GoldTopEdge", new Vector2(.025f, .985f), new Vector2(.975f, .991f), new Color(1f, .72f, .19f, .95f));
            MakeText(card.transform, "مرحباً بك في CORRUPT STATE", new Vector2(.07f, .82f), new Vector2(.93f, .95f), 34, new Color(1f, .79f, .31f), TextAnchor.MiddleCenter, true);
            MakeText(card.transform, "سجّل دخولك أو أنشئ حسابك وابدأ رحلتك", new Vector2(.07f, .75f), new Vector2(.93f, .82f), 21, new Color(.86f, .88f, .92f), TextAnchor.MiddleCenter, false);

            MakeText(card.transform, "البريد الإلكتروني أو رقم الهاتف", new Vector2(.08f, .69f), new Vector2(.92f, .745f), 19, new Color(.92f, .93f, .96f), TextAnchor.MiddleRight, false);
            MakeInput(card.transform, "example@email.com أو رقم الهاتف", false, new Vector2(.07f, .57f), new Vector2(.93f, .69f), out identifierField);

            MakeText(card.transform, "كلمة المرور", new Vector2(.08f, .51f), new Vector2(.92f, .565f), 19, new Color(.92f, .93f, .96f), TextAnchor.MiddleRight, false);
            MakeInput(card.transform, "أدخل كلمة المرور (8 أحرف على الأقل)", true, new Vector2(.07f, .39f), new Vector2(.93f, .51f), out passwordField);

            confirmFieldObject = MakeInput(card.transform, "أعد كتابة كلمة المرور", true, new Vector2(.07f, .22f), new Vector2(.93f, .34f), out confirmField);
            confirmFieldObject.SetActive(false);
            loginHintObject = MakeText(card.transform, "بياناتك تُحفظ بأمان وتُتحقق عبر خادم اللعبة", new Vector2(.07f, .25f), new Vector2(.93f, .34f), 18, new Color(.72f, .76f, .82f), TextAnchor.MiddleCenter, false).gameObject;

            submitButton = MakeButton(card.transform, "تسجيل الدخول", new Vector2(.07f, .105f), new Vector2(.93f, .215f), 27, new Color(.79f, .52f, .08f, 1f), new Color(.045f, .035f, .02f), Submit);
            submitText = submitButton.GetComponentInChildren<Text>();
            MakeButton(card.transform, "الدخول كضيف", new Vector2(.07f, .035f), new Vector2(.48f, .095f), 19, new Color(.09f, .105f, .13f, .98f), Color.white, Guest);
            var toggleButton = MakeButton(card.transform, "إنشاء حساب جديد", new Vector2(.52f, .035f), new Vector2(.93f, .095f), 19, new Color(.09f, .105f, .13f, .98f), new Color(1f, .78f, .28f), ToggleRegister);
            toggleText = toggleButton.GetComponentInChildren<Text>();
            statusText = MakeText(parent, "جاهز. أدخل بياناتك للمتابعة.", new Vector2(.55f, .045f), new Vector2(.95f, .095f), 18, Color.white, TextAnchor.MiddleCenter, false);
        }

        Image MakeImage(Transform parent, string name, Vector2 min, Vector2 max, Color color) {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image));
            go.transform.SetParent(parent, false);
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = min; rect.anchorMax = max; rect.offsetMin = Vector2.zero; rect.offsetMax = Vector2.zero;
            var image = go.GetComponent<Image>(); image.color = color;
            return image;
        }

        Text MakeText(Transform parent, string value, Vector2 min, Vector2 max, int size, Color color, TextAnchor anchor, bool bold) {
            var go = new GameObject("Text", typeof(RectTransform), typeof(Text));
            go.transform.SetParent(parent, false);
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = min; rect.anchorMax = max; rect.offsetMin = new Vector2(8, 4); rect.offsetMax = new Vector2(-8, -4);
            var text = go.GetComponent<Text>();
            text.font = font; text.text = ArabicFixer.Fix(value); text.fontSize = size; text.color = color;
            text.alignment = anchor; text.fontStyle = bold ? FontStyle.Bold : FontStyle.Normal;
            text.horizontalOverflow = HorizontalWrapMode.Wrap; text.verticalOverflow = VerticalWrapMode.Overflow;
            text.supportRichText = false; text.raycastTarget = false;
            return text;
        }

        GameObject MakeInput(Transform parent, string hint, bool password, Vector2 min, Vector2 max, out InputField field) {
            var go = new GameObject(password ? "PasswordField" : "TextField", typeof(RectTransform), typeof(Image), typeof(InputField));
            go.transform.SetParent(parent, false);
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = min; rect.anchorMax = max; rect.offsetMin = Vector2.zero; rect.offsetMax = Vector2.zero;
            var image = go.GetComponent<Image>(); image.color = new Color(.055f, .07f, .09f, .98f);
            field = go.GetComponent<InputField>(); field.targetGraphic = image; field.lineType = InputField.LineType.SingleLine;
            field.contentType = password ? InputField.ContentType.Password : InputField.ContentType.Standard;
            field.characterLimit = password ? 128 : 254;
            var textObject = new GameObject("InputText", typeof(RectTransform), typeof(Text));
            textObject.transform.SetParent(go.transform, false);
            var textRect = textObject.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero; textRect.anchorMax = Vector2.one; textRect.offsetMin = new Vector2(18, 8); textRect.offsetMax = new Vector2(-18, -8);
            var text = textObject.GetComponent<Text>(); text.font = font; text.fontSize = 22; text.color = Color.white;
            text.alignment = TextAnchor.MiddleRight; text.supportRichText = false; text.raycastTarget = false;
            field.textComponent = text;
            var placeholderObject = new GameObject("Placeholder", typeof(RectTransform), typeof(Text));
            placeholderObject.transform.SetParent(go.transform, false);
            var placeholderRect = placeholderObject.GetComponent<RectTransform>();
            placeholderRect.anchorMin = Vector2.zero; placeholderRect.anchorMax = Vector2.one; placeholderRect.offsetMin = new Vector2(18, 8); placeholderRect.offsetMax = new Vector2(-18, -8);
            var placeholder = placeholderObject.GetComponent<Text>(); placeholder.font = font; placeholder.text = ArabicFixer.Fix(hint);
            placeholder.fontSize = 19; placeholder.color = new Color(.64f, .68f, .73f, 1f); placeholder.alignment = TextAnchor.MiddleRight;
            placeholder.raycastTarget = false; placeholder.supportRichText = false; field.placeholder = placeholder;
            return go;
        }

        Button MakeButton(Transform parent, string label, Vector2 min, Vector2 max, int size, Color background, Color foreground, UnityEngine.Events.UnityAction action) {
            var go = new GameObject("Button_" + label, typeof(RectTransform), typeof(Image), typeof(Button));
            go.transform.SetParent(parent, false);
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = min; rect.anchorMax = max; rect.offsetMin = Vector2.zero; rect.offsetMax = Vector2.zero;
            var image = go.GetComponent<Image>(); image.color = background;
            var button = go.GetComponent<Button>(); button.targetGraphic = image; button.onClick.AddListener(action);
            var colors = button.colors; colors.normalColor = background; colors.highlightedColor = Color.Lerp(background, Color.white, .14f);
            colors.pressedColor = Color.Lerp(background, Color.black, .2f); colors.selectedColor = colors.highlightedColor;
            button.colors = colors;
            var labelObject = new GameObject("Label", typeof(RectTransform), typeof(Text));
            labelObject.transform.SetParent(go.transform, false);
            var labelRect = labelObject.GetComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero; labelRect.anchorMax = Vector2.one; labelRect.offsetMin = new Vector2(8, 3); labelRect.offsetMax = new Vector2(-8, -3);
            var text = labelObject.GetComponent<Text>(); text.font = font; text.text = ArabicFixer.Fix(label); text.fontSize = size;
            text.color = foreground; text.alignment = TextAnchor.MiddleCenter; text.fontStyle = FontStyle.Bold;
            text.raycastTarget = false; text.supportRichText = false;
            return button;
        }

        void ToggleRegister() {
            registering = !registering;
            confirmFieldObject.SetActive(registering);
            loginHintObject.SetActive(!registering);
            submitText.text = ArabicFixer.Fix(registering ? "إنشاء الحساب" : "تسجيل الدخول");
            toggleText.text = ArabicFixer.Fix(registering ? "لديك حساب؟ تسجيل الدخول" : "إنشاء حساب جديد");
            SetStatus(registering ? "أنشئ حسابك باستخدام البريد أو رقم الهاتف." : "أدخل بيانات حسابك للمتابعة.");
        }

        void ToggleEndpointSettings() {
            settingsOpen = !settingsOpen;
            endpointFieldObject.SetActive(settingsOpen);
            saveEndpointObject.SetActive(settingsOpen);
            endpointHint.gameObject.SetActive(settingsOpen);
            settingsButton.gameObject.SetActive(!settingsOpen);
        }

        void SaveEndpoint() {
            string value = (endpointField.text ?? "").Trim().TrimEnd('/');
            Uri uri;
            if (!Uri.TryCreate(value, UriKind.Absolute, out uri) || (uri.Scheme != Uri.UriSchemeHttps && uri.Scheme != Uri.UriSchemeHttp)) {
                SetStatus("رابط الخادم غير صحيح. استخدم https:// أو http://");
                return;
            }
            PlayerPrefs.SetString("CSRP_API_URL", value);
            PlayerPrefs.Save();
            var auth = GetAuth(); auth.Configure(value);
            settingsOpen = false;
            endpointFieldObject.SetActive(false); saveEndpointObject.SetActive(false);
            endpointHint.gameObject.SetActive(false); settingsButton.gameObject.SetActive(true);
            SetStatus("تم حفظ رابط الخادم على هذا الجهاز.");
        }

        void Submit() {
            string id = (identifierField.text ?? "").Trim();
            string password = passwordField.text ?? "";
            if (string.IsNullOrWhiteSpace(id)) { SetStatus("أدخل البريد الإلكتروني أو رقم الهاتف."); return; }
            if (password.Length < 8) { SetStatus("كلمة المرور يجب أن تكون 8 أحرف على الأقل."); return; }
            if (registering && password != (confirmField.text ?? "")) { SetStatus("تأكيد كلمة المرور غير مطابق."); return; }
            if (!HasEndpoint()) { SetStatus("افتح إعدادات الاتصال وأدخل رابط خادم اللعبة أولاً."); return; }
            SetStatus(registering ? "جارٍ إنشاء حسابك..." : "جارٍ تسجيل الدخول...");
            var auth = GetAuth();
            if (registering) StartCoroutine(auth.Register(id, password, AfterAuth));
            else StartCoroutine(auth.Login(id, password, AfterAuth));
        }

        void Guest() {
            if (!HasEndpoint()) { SetStatus("افتح إعدادات الاتصال وأدخل رابط خادم اللعبة أولاً."); return; }
            SetStatus("جارٍ إنشاء جلسة ضيف...");
            StartCoroutine(GetAuth().Guest(AfterAuth));
        }

        bool HasEndpoint() => !string.IsNullOrWhiteSpace(endpointField != null ? endpointField.text : PlayerPrefs.GetString("CSRP_API_URL", ""));

        AuthClient GetAuth() {
            var auth = FindFirstObjectByType<AuthClient>();
            if (auth == null) auth = new GameObject("AuthClient").AddComponent<AuthClient>();
            string url = endpointField != null ? endpointField.text : PlayerPrefs.GetString("CSRP_API_URL", DefaultApiUrl);
            if (!string.IsNullOrWhiteSpace(url)) auth.Configure(url);
            return auth;
        }

        void AfterAuth(bool ok, string error) {
            if (!ok) { SetStatus("تعذر إكمال العملية: " + error); Debug.LogWarning("Authentication failed: " + error); return; }
            SetStatus("تم التحقق من الحساب. جارٍ تحميل بيانات اللاعب...");
            var auth = GetAuth();
            StartCoroutine(auth.GetMe((json, requestError) => {
                if (requestError != null) { SetStatus("تم الدخول لكن تعذر تحميل بيانات اللاعب: " + requestError); Debug.LogWarning("Server /me failed: " + requestError); return; }
                Debug.Log("Authenticated player state received from /me.");
                SetStatus("نجح الدخول. جارٍ فتح الردهة...");
                SceneManager.LoadScene("LobbyScene");
            }));
        }

        void SetStatus(string message) {
            if (statusText != null) statusText.text = ArabicFixer.Fix(message);
            Debug.Log("[Login UI] " + message);
        }

        static void Stretch(RectTransform rect) {
            rect.anchorMin = Vector2.zero; rect.anchorMax = Vector2.one; rect.offsetMin = Vector2.zero; rect.offsetMax = Vector2.zero;
        }
    }
}