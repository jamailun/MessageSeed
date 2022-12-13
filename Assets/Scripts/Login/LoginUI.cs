using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LoginUI : MonoBehaviour {

	[SerializeField] private TMP_InputField username;	
	[SerializeField] private TMP_InputField password;
	[SerializeField] private Button button;

	[SerializeField] private TMP_Text errorDisplay;

	// Call from the Button component
	public void Button_Login() {
		ErrorDisplay(""); // clear error
		button.interactable = false;
		// Try to login.
		AccountManager.Instance.TryLogin(username.text, password.text, ErrorDisplay);
	}

	private void ErrorDisplay(string error) {
		if(errorDisplay)
			errorDisplay.text = error;
		button.interactable = true;
	}

}