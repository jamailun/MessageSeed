using UnityEngine;
using UnityEngine.UI;

public class SettingsWindow : MainMenuWindowUI {

	[Header("Internal references")]
	[SerializeField] private Slider messageSize;
	[SerializeField] private Slider avatarSize;
	[SerializeField] private Slider bufferRenderDistance;

	[Header("External references")]
	[SerializeField] private DynamicAvatar avatar;
	[SerializeField] private MessagesDisplayer messagesDisplayer;
	[SerializeField] private GoMap.GOMap map;

	private void Start() {
		// On start, load values from what was saved.
		avatarSize.value = LocalData.GetSizeAvatar();
		messageSize.value = LocalData.GetSizeMessages();
		bufferRenderDistance.value = LocalData.GetSizeBuffer();
		// apply
		ApplyValues();
	}

	public void SaveChanges() {
		// save datda
		LocalData.SaveSizes(avatarSize.value, messageSize.value, (int) bufferRenderDistance.value);
		// apply
		ApplyValues();
	}

	private void ApplyValues() {
		// messages
		messagesDisplayer.ChangeSize(Mathf.Clamp(messageSize.value, 0.05f, 3f));
		// avatar
		avatar.gameObject.transform.localScale = Mathf.Clamp(avatarSize.value, 0.1f, 3f) * Vector3.one;
		// renderer
		map.tileBuffer = Mathf.Clamp((int) bufferRenderDistance.value, 0, 3);
	}

}