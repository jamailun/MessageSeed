using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ProfileDisplayUI : MainMenuWindowUI {

	[Header("Profile display")]
	[SerializeField] private TMP_Text usernameField;
	[SerializeField] private TMP_Text levelField;
	[SerializeField] private TMP_Text likesField;
	[SerializeField] private Image expBar;

	[SerializeField] private TMP_Text likesReceivedField;
	[SerializeField] private TMP_Text likesGivenField;

	[Header("Message list")]
	[SerializeField] private RectTransform linesContainer;
	[SerializeField] private MessageLineUI linePrefab;
	[SerializeField] private GameObject emptyMessage;
	public MessageRaycast.MessageOpenedEvent messageOpenEvent;

	public void UpdateProfile() {
		// Global profile
		AccountManager.Instance.TryGetProfile(ProfileUpdated, ProfileFailed);
		// Messages list
		AccountManager.Instance.TryGetMyMessagesList(UpdateChildren);
	}

	private void ProfileUpdated(ProfileResponse profile) {
		// username
		usernameField.text = profile.author_name;
		// level
		levelField.text = "Level " + profile.level;
		expBar.fillAmount = Mathf.Clamp((float) ((profile.experience - profile.experience_previous) / (profile.experience_next - profile.experience_previous)), 0f, 1f);

		likesReceivedField.text = profile.likes_received_total + "";
		likesGivenField.text = profile.likes_received_total + "";
	}

	private void ProfileFailed(int errCode) {
		Debug.Log("Could NOT get profile: " + errCode);
	}

	public void UpdateChildren(IEnumerable<MessageListSerializer> msgs) {
		// delete old children
		linesContainer.DestroyChildren();
		// add new elements
		if(msgs == null) {
			Debug.LogError("Messages list was NULL");
			emptyMessage.SetActive(true);
			return;
		}
		// to list
		var list = new List<MessageListSerializer>(msgs);
		emptyMessage.SetActive(list.Count == 0);
		list.Sort((a, b) => a.like_count - b.like_count);
		foreach(var msg in msgs) {
			var line = Instantiate(linePrefab, linesContainer);
			line.SetData(new(msg), m => messageOpenEvent?.Invoke(m));
		}
	}

}