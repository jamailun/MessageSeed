using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MessageDisplay : MonoBehaviour {

	public static MessageDisplay Instance { get; private set; }
	private Message _message;
	public Message Message => _message;

	[Header("General configuration")]
	[SerializeField] private TMP_Text authorField;
	[SerializeField] private TMP_Text titleField;
	[SerializeField] private TMP_Text contentField;

	[SerializeField] private TMP_Text dateAgeField;
	[SerializeField] private TMP_Text timeRemainingField;

	[SerializeField] private Button backButton;

	[Header("Like system")]
	[SerializeField] private TMP_Text likesField;
	[SerializeField] private Button likeButton;
	[SerializeField] private Image likeImage;
	[SerializeField] private Color colorLiked;
	[SerializeField] private Color colorNotLiked;

	[Header("Events")]
	[SerializeField] private MessageLikedEvent likedEvent;


	private void Awake() {
		if(Instance) {
			Destroy(gameObject);
			return;
		}
	}

	public void PressedLike() {
		if(_message == null || _message.WasMessageLiked)
			return;
		Debug.Log("start liking.");
		backButton.interactable = false;
		likeButton.gameObject.SetActive(false);
		likedEvent?.Invoke(this);
	}

	public void EndOfLikeProcess() {
		backButton.interactable = true;
		Refresh();
	}

	public void SetMessage(Message message) {
		this._message = message;
		Refresh();
	}

	private void Refresh() {
		// General
		authorField.text = Message.AuthorName;
		titleField.text = Message.MessageTitle;
		contentField.text = Message.MessageContent;

		// Dates
		dateAgeField.text = DisplayDate(DateTime.UtcNow - Message.CreationTime);
		timeRemainingField.text = DisplayDate(Message.DeathTime - DateTime.UtcNow);

		// Likes
		likesField.text = DisplayLikes(Message.LikesAmount);
		likeButton.gameObject.SetActive((! Message.WasMessageLiked) && (Message.AuthorId != AccountManager.Account.accountId));
		likeImage.color = Message.WasMessageLiked ? colorLiked : colorNotLiked;
	}

	private string DisplayDate(TimeSpan span) {
		if(span.TotalSeconds < 0)
			return "invalid date.";
		if(span.TotalDays >= 1)
			return span.Days + " day" + (span.Days > 1 ? "s" : "");
		if(span.TotalHours >= 1)
			return span.Hours + " hour" + (span.Hours > 1 ? "s" : "");
		if(span.TotalMinutes >= 1)
			return span.Minutes + " minute" + (span.Minutes > 1 ? "s" : "");
		return span.Seconds + " second" + (span.Seconds > 1 ? "s" : "");
	}

	private string DisplayLikes(int amount) {
		if(amount > 1000)
			return (amount/1000)+ "K";
		return amount + "";
	}

	[Serializable]
	public class MessageLikedEvent : UnityEvent<MessageDisplay> { }

}
