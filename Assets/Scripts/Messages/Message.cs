﻿using UnityEngine;
using GoShared;

public class Message {

	public MessageHeader header;
	public bool IsComplete { get; private set; }
	public string MessageId => header.AuthorId;

	public string messageTitle;
	public string messageContent;

	public string authorName;

	public long deathTime; // timecode of the death
	public long likesAmount;

	public GoShared.Coordinates Coordinates => header.Coordinates;

	public Color MessageColor => AccountManager.IsMe(header.AuthorId) ? Color.yellow : Color.blue;

	// DEBUG constructor
	private Message(string id, string author, string t, string c, Coordinates center) {
		this.header = new() { id = id, author = author };
		authorName = author;
		messageTitle = t;
		messageContent = c;
		IsComplete = true;

		Vector2 delta = Random.insideUnitCircle * 1.1f;
		header.latitude = (float) center.latitude + delta.x;
		header.longitude = (float) center.longitude + delta.y;
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

	public static Message DebugMessage(int i, Coordinates center) {
		return new("test_" + i, "AUTHOR_TEST", "TEST_" + i, "Message de test n°" + i + ".", center);
	}
}

[System.Serializable]
public struct MessageHeader {
	public string id;
	public string author;
	public float latitude, longitude;

	public Coordinates Coordinates => new(latitude, longitude);

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

