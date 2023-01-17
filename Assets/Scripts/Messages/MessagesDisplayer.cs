using System.Collections.Generic;
using UnityEngine;
using GoMap;
using GoShared;

public class MessagesDisplayer : MonoBehaviour {

	private readonly Dictionary<string, MessageRenderer> renderers = new();
	[SerializeField] private MessagesManager messagesManager;
	[SerializeField] private MessageRenderer rendererPrefab;

	private Vector3 _scale = Vector3.zero;
	private float lastUpdate;
	private Coordinates lastCoordinates;

	public void PositionInit(Coordinates coordinates) {
		messagesManager.UpdatePosition(coordinates);
		messagesManager.UpdateMessages(coordinates);
		lastUpdate = Time.time;
		lastCoordinates = coordinates;
	}

	public void PositionChanged(Coordinates coordinates) {
		messagesManager.UpdatePosition(coordinates);
		// update after 120s || moved of 200m
		if(Time.time - lastUpdate >= 120 || coordinates.DistanceFromPoint(lastCoordinates) >= 200) {
			messagesManager.UpdateMessages(coordinates);
			lastUpdate = Time.time;
			lastCoordinates = coordinates;
		}
	}
	
	public void MessagesAdded(List<Message> messages) {
		if(_scale == Vector3.zero)
			_scale = LocalData.GetSizeMessages() * Vector3.one;
		foreach(var msg in messages) {
			if(renderers.ContainsKey(msg.MessageId)) {
				Debug.Log("ignoring " + msg);
				continue;
			}

			var renderer = Instantiate(rendererPrefab, transform);
			renderer.SetMessage(msg);
			renderer.gameObject.name = "MessageRenderer["+msg.MessageId+"]";
			renderer.transform.localPosition = msg.Coordinates.convertCoordinateToVector(0);
			renderer.transform.localScale = _scale;

			renderers.Add(msg.MessageId, renderer);
		}
	}

	public void ChangeSize(float newSize) {
		_scale = newSize * Vector3.one;
		foreach(var rdr in renderers.Values)
			rdr.transform.localScale = _scale;
	}

	public void TileLoaded(GOTile tile) {
		//Debug.Log("TILE " + tile.name + " loaded. center=" + tile.goTile.tileCenter);
	}
	
	
}