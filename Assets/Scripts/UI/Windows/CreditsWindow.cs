using UnityEngine;

public class CreditsWindow : MainMenuWindowUI {

	[Header("URL config")]
	[SerializeField] private string clientUrl = "https://github.com/jamailun/MessageSeed";
	[SerializeField] private string serverUrl = "https://github.com/JackyL56/MessageSeed_Django";

	public void Button_Client() {
		Application.OpenURL(clientUrl);
	}

	public void Button_Server() {
		Application.OpenURL(serverUrl);
	}

}