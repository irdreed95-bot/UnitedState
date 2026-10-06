using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using CorruptStateRP.Auth;

namespace CorruptStateRP.Onboarding {
    public sealed class OnboardingFlowController : MonoBehaviour {
        [SerializeField] AuthClient auth;
        [SerializeField] float splashSeconds=2.2f;
        [SerializeField] string loginScene="LoginScene";
        [SerializeField] string lobbyScene="LobbyScene";
        IEnumerator Start(){
            if(auth==null) auth=FindFirstObjectByType<AuthClient>();
            yield return new WaitForSeconds(splashSeconds);
            if(auth==null){SceneManager.LoadScene(loginScene);yield break;}
            bool restored=false; yield return auth.RestoreSession(ok=>restored=ok);
            SceneManager.LoadScene(restored?lobbyScene:loginScene);
        }
        public void OpenLogin(){SceneManager.LoadScene(loginScene);}
        public void OpenLobby(){SceneManager.LoadScene(lobbyScene);}
    }
}
