using UnityEngine;
namespace CorruptStateRP {
 public sealed class LoginUIController:MonoBehaviour {
  public void OnClickEmailLogin(string emailOrPhone,string password){if(string.IsNullOrWhiteSpace(emailOrPhone)||string.IsNullOrWhiteSpace(password)){Debug.LogWarning("Login requires credentials.");return;}Debug.Log("Credential login requested.");}
  public void OnClickFacebookLogin(){Debug.Log("Facebook login requested; provider integration will be connected in the online backend phase.");}
  public void OnClickPhoneLogin(string phone){if(!string.IsNullOrWhiteSpace(phone))Debug.Log("Phone login requested.");}
 }
}