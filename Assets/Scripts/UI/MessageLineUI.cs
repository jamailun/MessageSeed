using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MessageLineUI : MonoBehaviour {

	[SerializeField] private Image icon;
	[SerializeField] private TMP_Text title;
	[SerializeField] private TMP_Text water;

	[SerializeField] private Sprite iconDead;
	[SerializeField] private Sprite iconSeed;
	[SerializeField] private Sprite iconSapling;
	[SerializeField] private Sprite iconTree;

	public void SetData(MessageListSerializer msg) {
		title.text = msg.title;
		water.text = msg.likes_count + "";
		icon.sprite = GetIcon(msg.likes_count);
	}

	private Sprite GetIcon(int state) {
		return (state) switch {
			0 => iconSeed,
			1 => iconSapling,
			2 => iconTree,
			_ => iconDead
		};
	}

}