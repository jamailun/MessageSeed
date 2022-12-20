using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class Message {

	public MessageHeader header;
	public bool IsComplete { get; private set; }
	public string MessageId => header.AuthorId;

	public string messageTitle;
	public string messageContent;

	public string authorName;

	public long deathTime; // timecode of the death
	public long likesAmount;

	public Color MessageColor => AccountManager.IsMe(header.AuthorId) ? Color.yellow : Color.blue;

	// DEBUG constructor
	private Message(string id, string author, string t, string c) {
		this.header = new() { id = id, author = author };
		authorName = author;
		messageTitle = t;
		messageContent = c;
		IsComplete = true;
	}

	public Message(MessageHeader header) {
		this.header = header;
		IsComplete = false;
	}

	public void Complete(MessageComplete message) {
		if(IsComplete) {
			Debug.LogError("Tried to complete message " + this + " with " + message + ", but it was already completed !");
			return;
		}
		authorName = message.author_name;
		messageTitle = message.title;
		messageContent = message.content;
		likesAmount = message.likesAmount;
		IsComplete = true;
	}

	public Vector2 RealWorldPosition {
		set { header.longitude = value.x; header.latitude = value.y; }
		get { return new(header.longitude, header.latitude); }
	}

	public bool ExistsOnServer => header.id != null;

	public const float EARTH_RADIUS_KM = 6371f;
	// Code adapted from https://www.geeksforgeeks.org/program-distance-two-points-earth/
	public float GetDistanceRealWorld(Vector2 realWorldPosition) {
		float lon1 = realWorldPosition.x * Mathf.Deg2Rad;
		float lon2 = header.longitude * Mathf.Deg2Rad;
		float lat1 = realWorldPosition.y * Mathf.Deg2Rad;
		float lat2 = header.latitude * Mathf.Deg2Rad;

		// Haversine formula
		float dlon = lon2 - lon1;
		float dlat = lat2 - lat1;
		float a = Mathf.Pow(Mathf.Sin(dlat / 2), 2) + Mathf.Cos(lat1) * Mathf.Cos(lat2) * Mathf.Pow(Mathf.Sin(dlon / 2), 2);
		float c = 2 * Mathf.Asin(Mathf.Sqrt(a));

		return c * EARTH_RADIUS_KM;
	}

	public static Message DebugMessage(int i) {
		return new Message("test_" + i, "AUTHOR_TEST", "TEST_" + i, "Message de test n°" + i + ".");
	}
}

[System.Serializable]
public struct MessageHeader {
	public string id;
	public string author;
	public float latitude, longitude;

	public string AuthorId {
		get {
			if(author == null || !author.Contains("/"))
				return author;
			var t = author.Split("/");
			if(t.Length < 2)
				return author;
			return t[^2];
		}
	}

	public override string ToString() {
		return "MessageHeader{id=" + id + ", author_id=" + AuthorId + ", lat=" + latitude + ", long=" + longitude + "}";
	}
}

[System.Serializable]
public struct MessageComplete {
	public string title;
	public string content;
	public string author_name;
	public int likesAmount;
}

[System.Serializable]
public struct MessagesHeaderList {
	public MessageHeader[] list;
}

