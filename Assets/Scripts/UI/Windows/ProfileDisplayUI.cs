using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProfileDisplayUI : MainMenuWindowUI {

	[SerializeField] private TMP_Text usernameField;
	[SerializeField] private TMP_Text levelField;
	[SerializeField] private TMP_Text likesField;

	[SerializeField] private TMP_Text messagesAmountField;
	[SerializeField] private TMP_Text mostLikedMessageField;

	public void UpdateProfile() {
		AccountManager.Instance.TryGetProfile(ProfileUpdated, ProfileFailed);
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

}