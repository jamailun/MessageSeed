using UnityEngine;
using GoShared;

[System.Serializable]
public class Message {

	public MessageHeader header;
	public bool IsComplete { get; private set; }
	public string AuthorId => header.AuthorId;
	public string MessageId => header.id;

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

		Vector2 delta = Random.insideUnitCircle * 0.0005f;
		header.latitude = center.latitude + (double) delta.x;
		header.longitude = center.longitude + (double) delta.y;
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
		messageContent = message.message;
		likesAmount = message.likes_count;
		IsComplete = true;
	}

	public bool ExistsOnServer => header.id != null;

	public static Message DebugMessage(int i, Coordinates center) {
		return new("test_" + i, "AUTHOR_TEST", "TEST_" + i, "Message de test n°" + i + ".", center);
	}

	public override string ToString() {
		return "MSG{" + MessageId + ", title=" + messageTitle + ", pos=" + Coordinates + "}";
	}

}

[System.Serializable]
public struct MessageHeader {
	public string id;
	public string author;
	public double latitude, longitude;

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
	public string message;
	public double unix_post_date;
	public double unix_death_date;
	public string author_name;
	public int likes_count;

	public override string ToString() {
		return "MessageComplete{'" + title + "', auth=" + author_name + ", likes=" + likes_count + "}";
	}
}

[System.Serializable]
public struct MessagesHeaderList {
	public MessageHeader[] list;
}

