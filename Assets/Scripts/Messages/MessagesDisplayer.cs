using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoMap;
using GoShared;

public class MessagesDisplayer : MonoBehaviour {

	private readonly List<MessageRenderer> renderers = new();
	[SerializeField] private MessageRenderer rendererPrefab;

	public void PositionChanged(Coordinates coordinates) {
		
	}

	public void MessagesAdded(List<Message> messages) {
		foreach(var msg in messages) {
			var renderer = Instantiate(rendererPrefab, transform);
			renderer.SetMessage(msg);
			renderer.transform.SetPositionAndRotation(msg.Coordinates.convertCoordinateToVector(), Quaternion.identity);
		}
	}

	public void TileLoaded(GOTile tile) {
		Debug.Log("TILE " + tile.name + " loaded. center=" + tile.goTile.tileCenter);
	}
	
	
}