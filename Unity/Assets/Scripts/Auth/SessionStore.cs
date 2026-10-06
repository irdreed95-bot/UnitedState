using UnityEngine;

namespace CorruptStateRP.Auth {
    public interface ISessionStore { SessionTokens Load(); void Save(SessionTokens tokens); void Clear(); }

    // Development-safe abstraction. Production Android builds should swap this implementation
    // for an Android Keystore-backed store before shipping credentials.
    public sealed class PlayerPrefsSessionStore : ISessionStore {
        const string Key="corrupt_state_session_v1";
        public SessionTokens Load(){
            string json=PlayerPrefs.GetString(Key,string.Empty);
            if(string.IsNullOrEmpty(json)) return null;
            try{return JsonUtility.FromJson<SessionTokens>(json);}catch{return null;}
        }
        public void Save(SessionTokens tokens){
            PlayerPrefs.SetString(Key,JsonUtility.ToJson(tokens)); PlayerPrefs.Save();
        }
        public void Clear(){PlayerPrefs.DeleteKey(Key);PlayerPrefs.Save();}
    }
}
