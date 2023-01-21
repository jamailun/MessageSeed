using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class NewMessagePanel : MonoBehaviour {

	[SerializeField] private TMP_InputField headerInput;
	[SerializeField] private TMP_InputField contentInput;
	[SerializeField] private Button saveButton;
	[SerializeField] private UnityEvent afterSaveEvent;

	private void Start() {
		saveButton.interactable = true;
	}

	public void UI_SavePressed() {
		saveButton.interactable = false;
		// Get inputs content
		var header = headerInput.text;
		var content = contentInput.text;

		// Validation
		var error = Validate(header, content);
		if(error != null) {
			Debug.LogError("Validation error : " + error);
			saveButton.interactable = true;
			AlertUI.OpenAlert(error);
			return;
		}

		// Accept
		Debug.Log("Sending new message : (" + header + ") : '" + content + "'.");
		MessagesManager.Instance.WriteMessage(header, content, MessageSendingError, MessageSendingOver);
	}

	private void MessageSendingError(string error) {
		Debug.LogError("ERROR : could NOT send message. Error is \"" + error + "\".");
		AlertUI.OpenAlert(error);
	}

	private void MessageSendingOver() {
		// clear
		headerInput.text = "";
		contentInput.text = "";
		// close
		afterSaveEvent?.Invoke();
		saveButton.interactable = true;
	}

	private string Validate(string header, string content) {
		if(header.Length < 3)
			return "Message title is too short";
		if(header.Length > 35)
			return "Message title is too long";
		if(content.Length < 3)
			return "Message content is too short";
		if(content.Length > 1000)
			return "Message content is too long";
		// More validations
		return null;
	}
	
}