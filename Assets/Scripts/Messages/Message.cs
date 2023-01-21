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
	public MessageState State => header.State;
	// Times
	public DateTime CreationTime { get; private set; }
	public DateTime DeathTime { get; private set; }
	// Likes
	public int LikesAmount { get; private set; }
	public bool WasMessageLiked { get; private set; }
	// Other
	public Coordinates Coordinates => header.Coordinates;

	public Color MessageColor => AccountManager.IsMe(header.AuthorId) ? Color.yellow : Color.blue;

	public Message(MessageHeader header) {
		this.header = header;
		IsComplete = false;
	}

	public Message(MessageListSerializer serializer) {
		header = serializer.ToHeader();
		Complete(serializer.ToComplete());
	}

	public void Complete(MessageComplete message) {
		AuthorName = message.author_name;
		MessageTitle = message.title;
		MessageContent = message.message;

		CreationTime = TimeUtils.UnixTimeStampToDateTime(message.unix_post_date);
		DeathTime = TimeUtils.UnixTimeStampToDateTime(message.unix_death_date);

		LikesAmount = message.like_count;
		WasMessageLiked = message.me_liked;

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

public enum MessageState : int {
	Seed = 0,
	Sapling = 1,
	Tree = 2,
	Dead = 3,
	Error = -1
}

[System.Serializable]
public struct MessageHeader {
	public string id;
	public string author;
	public double latitude, longitude;
	public int state;

	public MessageState State {
		get {
			foreach(MessageState s in Enum.GetValues(typeof(MessageState)))
				if((int)s == state)
					return s;
			return MessageState.Error;
		}
	}
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
	public bool me_liked;

	public override string ToString() {
		return "MessageComplete{'" + title + "', auth=" + author_name + ", likes=" + like_count + "}";
	}
}

[System.Serializable]
public struct MessagesHeaderList {
	public MessageHeader[] list;
}


[System.Serializable]
public struct MessageListSerializer {
	// header
	public string id;
	public string author;
	public double latitude, longitude;
	public int state;
	// now
	public string title;
	public string message;
	public string author_name;
	public int like_count;
	public double unix_post_date;
	public double unix_death_date;
	public bool me_liked;

	public MessageHeader ToHeader() {
		return new MessageHeader() {
			id = id,
			author = author,
			latitude = latitude,
			longitude = longitude,
			state = state
		};
	}

	public MessageComplete ToComplete() {
		return new MessageComplete() {
			author_name = author_name,
			like_count = like_count,
			message = message,
			title = title,
			me_liked = me_liked,
			unix_death_date = unix_death_date,
			unix_post_date = unix_post_date
		};
	}
}

[System.Serializable]
public struct MessageListSerializerList {
	public string user_id;
	public MessageListSerializer[] my_messages;
}
