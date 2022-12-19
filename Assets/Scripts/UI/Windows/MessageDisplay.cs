using TMPro;
using UnityEngine;

public class MessageDisplay : MonoBehaviour {

	public static MessageDisplay Instance { get; private set; }

	[SerializeField] private TMP_Text authorField;
	[SerializeField] private TMP_Text titleField;
	[SerializeField] private TMP_Text contentField;
	[SerializeField] private TMP_Text likesField;

	private void Awake() {
		if(Instance) {
			Destroy(gameObject);
			return;
		}
	}

	public void test() {
		SetMessage(Message.DebugMessage(12));
	}

	public void SetMessage(Message message) {
		authorField.text = message.authorName;
		titleField.text = message.messageTitle;
		contentField.text = message.messageContent;
		likesField.text = DisplayLikes(message.likesAmount);
	}

	private string DisplayLikes(long amount) {
		if(amount > 1000)
			return (amount/1000)+ "K";
		return amount + "";
	}

}
