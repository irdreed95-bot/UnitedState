using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace CorruptStateRP.Auth {
    public sealed class AuthClient : MonoBehaviour {
        [SerializeField] string baseUrl="";
        [SerializeField] string loginPath="/auth/login";
        [SerializeField] string refreshPath="/auth/refresh";
        ISessionStore store;
        public SessionTokens Session {get;private set;}
        public bool HasSession => Session != null && Session.IsUsable;
        public event Action<bool> SessionChanged;
        void Awake(){ store=new PlayerPrefsSessionStore(); Session=store.Load(); }
        public void Configure(string url){baseUrl=(url??string.Empty).TrimEnd('/');}
        public IEnumerator RestoreSession(Action<bool> completed=null){
            if(!HasSession){completed?.Invoke(false);yield break;}
            if(!Session.AccessTokenExpired){SessionChanged?.Invoke(true);completed?.Invoke(true);yield break;}
            yield return Refresh(completed);
        }
        public IEnumerator Login(string identifier,string password,Action<bool,string> completed=null){
            yield return PostJson(loginPath,JsonUtility.ToJson(new LoginRequest{identifier=identifier,password=password}),
                json=>{Session=JsonUtility.FromJson<SessionTokens>(json);store.Save(Session);SessionChanged?.Invoke(true);completed?.Invoke(true,null);},
                err=>completed?.Invoke(false,err));
        }
        public IEnumerator Refresh(Action<bool> completed=null){
            if(!HasSession){completed?.Invoke(false);yield break;}
            yield return PostJson(refreshPath,JsonUtility.ToJson(new RefreshRequest{refreshToken=Session.refreshToken}),
                json=>{Session=JsonUtility.FromJson<SessionTokens>(json);store.Save(Session);SessionChanged?.Invoke(true);completed?.Invoke(true);},
                err=>{store.Clear();Session=null;SessionChanged?.Invoke(false);completed?.Invoke(false);});
        }
        public void Logout(){store.Clear();Session=null;SessionChanged?.Invoke(false);}
        IEnumerator PostJson(string path,string body,Action<string> success,Action<string> failure){
            if(string.IsNullOrWhiteSpace(baseUrl)){failure?.Invoke("Auth backend URL is not configured.");yield break;}
            using(var req=new UnityWebRequest(baseUrl+path,"POST")){
                byte[] data=System.Text.Encoding.UTF8.GetBytes(body); req.uploadHandler=new UploadHandlerRaw(data); req.downloadHandler=new DownloadHandlerBuffer();
                req.SetRequestHeader("Content-Type","application/json"); req.timeout=15;
                yield return req.SendWebRequest();
                if(req.result==UnityWebRequest.Result.Success) success?.Invoke(req.downloadHandler.text); else failure?.Invoke(req.error);
            }
        }
        [Serializable] struct LoginRequest{public string identifier;public string password;}
        [Serializable] struct RefreshRequest{public string refreshToken;}
    }
}
