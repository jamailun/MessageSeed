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
			//TODO diusplay error to UI
			return;
		}

		// Accept
		Debug.Log("Sending new message : (" + header + ") : '" + content + "'.");
		MessagesManager.Instance.WriteMessage(header, content, MessageSendingError, MessageSendingOver);
	}

	private void MessageSendingError(string error) {
		// TODO
	}
	private void MessageSendingOver() {
		// clear
		headerInput.text = "";
		contentInput.text = "";
		// close
		afterSaveEvent?.Invoke();
		saveButton.interactable = true;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="header"></param>
	/// <param name="content"></param>
	/// <returns></returns>
	private string Validate(string header, string content) {
		if(header.Length < 5)
			return "Message title is too short";
		if(content.Length < 15)
			return "Message content is too short";
		// More validations
		return null;
	}
	
}