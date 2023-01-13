using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MessageLineUI : MonoBehaviour {

	private Message _message;
	private ProfileDisplayUI profileDisplay;

	[SerializeField] private Image icon;
	[SerializeField] private TMP_Text title;
	[SerializeField] private TMP_Text water;

	[SerializeField] private Sprite iconDead;
	[SerializeField] private Sprite iconSeed;
	[SerializeField] private Sprite iconSapling;
	[SerializeField] private Sprite iconTree;

	private void Start() {
		profileDisplay = GetComponentInParent<ProfileDisplayUI>();
	}

	public void SetData(MessageListSerializer msg) {
		// data
		_message = new Message(msg);
		// display
		title.text = msg.title;
		water.text = msg.like_count + "";
		icon.sprite = GetIcon(msg.like_count);
	}

	private Sprite GetIcon(int state) {
		return (state) switch {
			0 => iconSeed,
			1 => iconSapling,
			2 => iconTree,
			_ => iconDead
		};
	}

	public void LineClicked() {
		Debug.Log("OK CLICKED");
		profileDisplay.messageOpenEvent?.Invoke(_message);
	}

}