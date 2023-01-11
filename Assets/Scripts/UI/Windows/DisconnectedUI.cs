using UnityEngine;
using UnityEngine.SceneManagement;

public class DisconnectedUI : MonoBehaviour {

	public void PressedLeave() {
		AccountManager.Instance.InvalidateSession();
		SceneManager.LoadScene("LoginScene");
	}
		
}