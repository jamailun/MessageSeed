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
		messagesManager.UpdateMessages(coordinates);
	}

	public void PositionChanged(Coordinates coordinates) {
		// compute distance and stuff	
	//Debug.Log("POSITION CHANGED " + coordinates);
		//messagesManager.UpdateMessages(coordinates);
	}

	public void MessagesAdded(List<Message> messages) {
		Debug.LogWarning("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
		foreach(var msg in messages) {
			if(renderers.ContainsKey(msg.MessageId))
				continue;

			var renderer = Instantiate(rendererPrefab, transform);
			renderer.SetMessage(msg);
			renderer.transform.SetPositionAndRotation(msg.Coordinates.convertCoordinateToVector(), Quaternion.identity);

			renderers.Add(msg.MessageId, renderer);

			Debug.Log("DISPLAY NEW MESSAGE " + msg);
		}
	}

	public void TileLoaded(GOTile tile) {
		Debug.Log("TILE " + tile.name + " loaded. center=" + tile.goTile.tileCenter);
	}
	
	
}