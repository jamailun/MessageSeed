using UnityEngine;

public class LogoutUI : MonoBehaviour {

	public void Button_Yes() {
		AccountManager.Instance.TryLogout();
	}

	public void Button_No() {
		gameObject.SetActive(false);
	}

}