using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoginUI : MonoBehaviour {

	[SerializeField] private TMP_InputField username;	
	[SerializeField] private TMP_InputField password;
	[SerializeField] private Button acceptButton;

	[SerializeField] private Button goToSignin;
	[SerializeField] private TMP_Text errorDisplay;

	// Call from the Button component
	public void Button_Login() {
		ErrorDisplay(""); // clear error
		acceptButton.interactable = false;
		goToSignin.interactable = false;
		// Try to login.
		AccountManager.Instance.TryLogin(username.text, password.text, ErrorDisplay);
	}

	private void ErrorDisplay(string error) {
		if(errorDisplay)
			errorDisplay.text = error;
		acceptButton.interactable = true;
		goToSignin.interactable = true;
	}

}