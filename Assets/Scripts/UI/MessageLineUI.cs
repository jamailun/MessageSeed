using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MessageLineUI : MonoBehaviour {

	private Message _message;
	private CSharpExtension.Consumable<Message> _clickAction;

	[SerializeField] private Image icon;
	[SerializeField] private TMP_Text title;
	[SerializeField] private TMP_Text water;

	[SerializeField] private Sprite iconDead;
	[SerializeField] private Sprite iconSeed;
	[SerializeField] private Sprite iconSapling;
	[SerializeField] private Sprite iconTree;

	public void SetData(Message message, CSharpExtension.Consumable<Message> clickAction) {
		// data
		_message = message;
		_clickAction = clickAction;
		// display
		title.text = message.MessageTitle;
		water.text = message.LikesAmount + "";
		icon.sprite = GetIcon(message.State);
	}

	private Sprite GetIcon(MessageState state) {
		return (state) switch {
			MessageState.Seed => iconSeed,
			MessageState.Sapling => iconSapling,
			MessageState.Tree => iconTree,
			_ => iconDead
		};
	}

	public void LineClicked() {
		Debug.Log("OK CLICKED + " + _message);
		_clickAction?.Invoke(_message);
	}

}