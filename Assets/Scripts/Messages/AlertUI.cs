using UnityEngine;

public class AlertUI : MonoBehaviour {

	private static AlertUI Instance;

	private void Awake() {
		if(Instance) {
			Destroy(gameObject);
			return;
		}
		Instance = this;
	}

	[SerializeField] private TMPro.TMP_Text title;

	private void Open(string error) {
		gameObject.SetActive(true);
		title.text = error;
	}

	public void Button_Close() {
		gameObject.SetActive(false);
	}

	public static void OpenAlert(string errorMessage) {
		Instance.Open(errorMessage);
	}

}