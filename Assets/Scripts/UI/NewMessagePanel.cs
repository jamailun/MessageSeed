using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NewMessagePanel : MonoBehaviour {

	[SerializeField] private TMP_InputField headerInput;
	[SerializeField] private TMP_InputField contentInput;
	[SerializeField] private Button saveButton;

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
		Debug.Log("New message : (" + header + ") : '" + content + "'.");
		//TODO create the message (other branch)
		//TODO hide this windows by calling (indirectly) the #CloseEverything method on MainMenuUI (use a UnityEvent (in namespace UnityEditor.Events))
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