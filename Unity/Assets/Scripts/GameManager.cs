using UnityEngine;using UnityEngine.UI;
namespace CorruptStateRP {
public sealed class GameManager:MonoBehaviour{
 PlayerController player;VehicleController vehicle;Text stats,mission;int money=2500,bank=10000,xp,level=1,wanted;float health=100,hunger=100,thirst=100;
 readonly string[] missions={"إكمال تسجيل المواطن","فتح حساب بنكي","استخراج رخصة القيادة","شراء أول مركبة","اختيار وظيفة","زيارة مركز الشرطة","زيارة المستشفى","زيارة المحكمة","التعرف على منطقة العصابات","إكمال أول مهمة عمل","شراء منزل أو شقة","بناء سمعة داخل المدينة"};
 void Start(){var w=new GameObject("World");w.AddComponent<WorldBuilder>().Build();BuildPlayer();BuildVehicle();new GameObject("Traffic").AddComponent<TrafficManager>();BuildHUD();}
 void Update(){hunger=Mathf.Max(0,hunger-Time.deltaTime*.01f);thirst=Mathf.Max(0,thirst-Time.deltaTime*.018f);if(Input.GetKeyDown(KeyCode.F))ToggleVehicle();if(Input.GetKeyDown(KeyCode.R))vehicle.ToggleEngine();HUD();}
 void FixedUpdate(){var i=new Vector2(Input.GetAxis("Horizontal"),Input.GetAxis("Vertical"));if(player.gameObject.activeSelf)player.SetMoveInput(i,Input.GetKey(KeyCode.LeftShift));if(vehicle.Controlled)vehicle.Drive(i);}
 void BuildPlayer(){var go=new GameObject("Player");go.transform.position=new(0,1.2f,28);go.AddComponent<CharacterController>();player=go.AddComponent<PlayerController>();var cam=new GameObject("Main Camera").AddComponent<Camera>();cam.fieldOfView=68;cam.transform.SetParent(go.transform);cam.transform.localPosition=new(0,6.5f,10.5f);cam.transform.LookAt(go.transform.position+Vector3.up);}
 void BuildVehicle(){var go=new GameObject("PlayerVehicle");go.transform.position=new(0,1,12);vehicle=go.AddComponent<VehicleController>();vehicle.Initialize();}
 void ToggleVehicle(){vehicle.SetControlled(!vehicle.Controlled);player.gameObject.SetActive(!vehicle.Controlled);if(!vehicle.Controlled)player.transform.position=vehicle.transform.position+Vector3.right*2;}
 void BuildHUD(){var c=new GameObject("HUD").AddComponent<Canvas>();c.renderMode=RenderMode.ScreenSpaceOverlay;var p=new GameObject("Stats").AddComponent<Image>();p.transform.SetParent(c.transform);var r=p.rectTransform;r.anchorMin=new(0,1);r.anchorMax=new(0,1);r.pivot=new(0,1);r.anchoredPosition=new(18,-18);r.sizeDelta=new(430,145);r.color=new(.05f,.07f,.09f,.88f);stats=Text(p.transform,new(16,-12),new(400,95),18);mission=Text(p.transform,new(16,-108),new(400,30),15);}
 Text Text(Transform p,Vector2 pos,Vector2 size,int fs){var g=new GameObject("Text");g.transform.SetParent(p);var t=g.AddComponent<Text>();t.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");t.fontSize=fs;t.color=Color.white;t.rectTransform.anchoredPosition=pos;t.rectTransform.sizeDelta=size;return t;}
 void HUD(){stats.text=$"CORRUPT STATE RP\n$ {money} | بنك {bank} | LV {level} | XP {xp}\n❤️ {health:0}%  🍖 {hunger:0}%  💧 {thirst:0}%  ⭐ {wanted}";mission.text=$"المهمة: {missions[xp%missions.Length]}";}
}}
