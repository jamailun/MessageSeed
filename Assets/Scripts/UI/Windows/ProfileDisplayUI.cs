using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ProfileDisplayUI : MainMenuWindowUI {

	[Header("Profile display")]
	[SerializeField] private TMP_Text usernameField;
	[SerializeField] private TMP_Text levelField;
	[SerializeField] private TMP_Text likesField;

	[SerializeField] private TMP_Text messagesAmountField;
	[SerializeField] private TMP_Text mostLikedMessageField;

	[Header("Message list")]
	[SerializeField] private RectTransform linesContainer;
	[SerializeField] private MessageLineUI linePrefab;

	public void UpdateProfile() {
		// Global profile
		AccountManager.Instance.TryGetProfile(ProfileUpdated, ProfileFailed);
		// Messages list
		AccountManager.Instance.TryGetMessagesList(UpdateChildren);
	}

	private void ProfileUpdated(ProfileResponse profile) {
		usernameField.text = profile.author_name;
		levelField.text = "Level " + profile.level;
		likesField.text = profile.likes_received_total + "";
		messagesAmountField.text = profile.messages.Length + "";
		//mostLikedMessageField.text = profile.most
	}

	private void ProfileFailed(int errCode) {
		Debug.Log("Could NOT get profile: " + errCode);
	}

	public void UpdateChildren(IEnumerable<MessageListSerializer> msgs) {
		// delete old children
		linesContainer.DestroyChildren();
		// add new elements
		foreach(var msg in msgs) {
			var line = Instantiate(linePrefab, linesContainer);
			line.SetData(msg);
		}
	}

}