using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SigninUI : MonoBehaviour {

	[SerializeField] private TMP_InputField username;	
	[SerializeField] private TMP_InputField email;
	[SerializeField] private TMP_InputField password;
	[SerializeField] private Button acceptButton;

	[SerializeField] private Button goToLogin;
	[SerializeField] private TMP_Text errorDisplay;

	// Call from the Button component
	public void Button_SignIn() {
		ErrorDisplay(""); // clear error
		acceptButton.interactable = false;
		goToLogin.interactable = false;
		// Try to login.
		AccountManager.Instance.TrySignIn(username.text, email.text, password.text, ErrorDisplay);
	}

	private void ErrorDisplay(string error) {
		if(errorDisplay)
			errorDisplay.text = error;
		acceptButton.interactable = true;
		goToLogin.interactable = true;
	}

}