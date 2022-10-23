using UnityEngine;

/// <summary>
/// Serialized from JSON.
/// </summary>
[System.Serializable]
public class Message {

	public long messageId; // Comme discord, le timestamp == le message id

	public string messageTitle;

	public string messageContent;

	public string authorName;

	public string authorId;

	public long deathTime; // timecode of the death

	public long likesAmount;

	public Color messageColor => AccountManager.IsMe(authorId) ? Color.yellow : Color.blue;

	public Message() { /* JSON constructor */ }

	public Message(Account author, string title, string content) {
		messageTitle = title;
		messageContent = content;
		authorName = author.username;
		authorId = author.accountId;
	}

}