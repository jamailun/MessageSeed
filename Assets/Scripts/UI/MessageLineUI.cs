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
			1 => iconDead,
			2 => iconSeed,
			3 => iconSapling,
			4 => iconTree,
			_ => iconTree
		};
	}

}