using System.Collections;
using UnityEngine;
using TMPro;

public class LoginUI : MonoBehaviour {

	[SerializeField] private TMP_InputField username;	
	[SerializeField] private TMP_InputField password;

	// Call from the Button component
	public void Button_Login() {
		AccountManager.Instance.TryLogin(username.text, password.text);
	}

}