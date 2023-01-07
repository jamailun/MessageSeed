using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoMap;
using GoShared;

public class MessagesDisplayer : MonoBehaviour {

	private readonly Dictionary<string, MessageRenderer> renderers = new();
	[SerializeField] private MessagesManager messagesManager;
	[SerializeField] private MessageRenderer rendererPrefab;

	public void PositionInit(Coordinates coordinates) {
		Debug.LogWarning("POSITION INIT " + coordinates);
		messagesManager.UpdatePosition(coordinates);
		messagesManager.UpdateMessages(coordinates);
	}

	public void PositionChanged(Coordinates coordinates) {
		messagesManager.UpdatePosition(coordinates);
		//TODO IF DISTANCE LRAGE? UPDATE
		//messagesManager.UpdateMessages(coordinates);
	}
	
	public void MessagesAdded(List<Message> messages) {
		foreach(var msg in messages) {
			if(renderers.ContainsKey(msg.MessageId)) {
				Debug.Log("ignoring " + msg);
				continue;
			}

			var renderer = Instantiate(rendererPrefab, transform);
			renderer.SetMessage(msg);
			renderer.gameObject.name = "MessageRenderer["+msg.MessageId+"]";
			renderer.transform.localPosition = msg.Coordinates.convertCoordinateToVector(0);

			renderers.Add(msg.MessageId, renderer);
		}
	}

	public void TileLoaded(GOTile tile) {
		//Debug.Log("TILE " + tile.name + " loaded. center=" + tile.goTile.tileCenter);
	}
	
	
}