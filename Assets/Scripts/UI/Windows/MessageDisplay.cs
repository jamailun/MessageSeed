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

	private bool _loading = false;
	public void PressedLike() {
		if(_message == null || _message.WasMessageLiked || _loading)
			return;
		Debug.Log("start liking.");
		_loading = true;
		backButton.interactable = false;
		likeButton.interactable = false;
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
		likeButton.interactable = ! Message.WasMessageLiked;
		likeImage.color = Message.WasMessageLiked ? colorLiked : colorNotLiked;
	}

	private string DisplayDate(TimeSpan span) {
		if(span.TotalSeconds < 0)
			return "invalid date.";
		string s = "";
		if(span.TotalDays >= 1)
			s += span.Days + " day" + (span.Days > 1 ? "s" : "") + ", ";
		if(span.TotalHours >= 1)
			s += span.Hours + " hour" + (span.Hours > 1 ? "s" : "") + " and ";
		s += span.Minutes + " minute" + (span.Minutes > 1 ? "s" : "") + ".";
		return s;
	}

	private string DisplayLikes(int amount) {
		if(amount > 1000)
			return (amount/1000)+ "K";
		return amount + "";
	}

	[Serializable]
	public class MessageLikedEvent : UnityEvent<MessageDisplay> { }

}
