using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class ServerUnreachableUI : MonoBehaviour {

	[SerializeField] private GameObject loginUi;
	[SerializeField] private GameObject signinUi;
	[SerializeField] private GameObject commonUi;

	[SerializeField] private TMP_Text displayMessage;

	[SerializeField] private Button reloadButton;
	[SerializeField] private bool changeScene = false;



	private void OnEnable() {
		if(loginUi)
			loginUi.SetActive(false);
		if(signinUi)
			signinUi.SetActive(false);
		if(commonUi)
			commonUi.SetActive(false);
	}

	private void OnDisable() {
		if(loginUi)
			loginUi.SetActive(true);
		if(signinUi)
			signinUi.SetActive(false);
		if(commonUi)
			commonUi.SetActive(true);
	}

	public void SetError(string error) {
		displayMessage.text = error;
	}

	public void Button_Reload() {
		displayMessage.text = "Loading...";
		reloadButton.interactable = false;
		StartCoroutine(RemoteApiManager.CR_PingServer(PingSuccedeed, PingFailed));
	}

	private void PingFailed(string error) {
		displayMessage.text = error;
		reloadButton.interactable = true;
	}

	private void PingSuccedeed() {
		reloadButton.interactable = true;
		if(changeScene) {
			SceneManager.LoadScene("LoginScene");
		} else {
			gameObject.SetActive(false);
		}
	}

}