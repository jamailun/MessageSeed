using System;
using UnityEngine;
using GoShared;

[System.Serializable]
public class Message {

	private MessageHeader header;
	public bool IsComplete { get; private set; }

	// General
	public string AuthorId => header.AuthorId;
	public string MessageId => header.id;
	public string AuthorName { get; private set; }
	public string MessageTitle { get; private set; }
	public string MessageContent { get; private set; }
	// Times
	public DateTime CreationTime { get; private set; }
	public DateTime DeathTime { get; private set; }
	// Likes
	public int LikesAmount { get; private set; }
	public bool WasMessageLiked { get; private set; }//  => true; // TODO !!
	// Other
	public Coordinates Coordinates => header.Coordinates;

	public Color MessageColor => AccountManager.IsMe(header.AuthorId) ? Color.yellow : Color.blue;

	public Message(MessageHeader header) {
		this.header = header;
		IsComplete = false;
	}

	public void Complete(MessageComplete message) {
		if(IsComplete) {
			Debug.LogError("Tried to complete message " + this + " with " + message + ", but it was already completed !");
			return;
		}
		AuthorName = message.author_name;
		MessageTitle = message.title;
		MessageContent = message.message;

		CreationTime = TimeUtils.UnixTimeStampToDateTime(message.unix_post_date);
		DeathTime = TimeUtils.UnixTimeStampToDateTime(message.unix_death_date);

		LikesAmount = message.like_count;
		WasMessageLiked = false;

		IsComplete = true;
	}

	public void SetAsLiked() {
		LikesAmount++;
		WasMessageLiked = true;
	}

	public bool ExistsOnServer => header.id != null;

	public override string ToString() {
		return "MSG{" + MessageId + ", title=" + MessageTitle + ", pos=" + Coordinates + "}";
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
	public int like_count;

	public override string ToString() {
		return "MessageComplete{'" + title + "', auth=" + author_name + ", likes=" + like_count + "}";
	}
}

[System.Serializable]
public struct MessagesHeaderList {
	public MessageHeader[] list;
}

