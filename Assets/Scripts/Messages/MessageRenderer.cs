using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class MessageRenderer : MonoBehaviour {

	private Message _message;
	private SpriteRenderer _spriteRenderer; //todo

	public bool HasMessage => _message != null;
	public Message Message => _message;

	public void SetMessage(Message message) {
		this._message = message;
		if(message != null)
			GetComponent<SpriteRenderer>().color = message.MessageColor;
	}


}