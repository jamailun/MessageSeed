using UnityEngine;

public class AlertUI : MonoBehaviour {

	[SerializeField] private TMPro.TMP_Text title;

	public void Open(string error) {
		gameObject.SetActive(true);
		title.text = error;
	}

	public void Button_Close() {
		gameObject.SetActive(false);
	}

}