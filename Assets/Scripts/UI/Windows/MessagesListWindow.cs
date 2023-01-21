using UnityEngine;
using System.Collections.Generic;

public class MessagesListWindow : MainMenuWindowUI {

	public static MessagesListWindow Instance { get; private set; }

	private void Awake() {
		if(Instance) {
			Destroy(gameObject);
			return;
		}
		Instance = this;
	}

	[SerializeField] private RectTransform content;
	[SerializeField] private MessageLineUI linePrefab;

	public void OpenWithMessages(IEnumerable<MessageRenderer> renderers, CSharpExtension.Consumable<Message> chosenEvent) {
		GetComponentInParent<MainMenuUI>().TryOpen(this);
		// clear
		content.DestroyChildren();
		// add children
		foreach(var renderer in renderers) {
			var line = Instantiate(linePrefab, content);
			line.SetData(renderer.Message, m => {
				Close();
				chosenEvent?.Invoke(m);
			});
		}
	}

}