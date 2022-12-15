using System.Collections;
using UnityEngine;

public class MessageRenderer : MonoBehaviour {

	private Message _message;
	private SpriteRenderer _spriteRenderer;

	public bool HasMessage => _message != null;
	public Message Message => _message;

	private void Start() {
		if(!_spriteRenderer)
			_spriteRenderer = GetComponent<SpriteRenderer>();
	}

	public void SetMessage(Message message) {
		this._message = message;
		if(message != null)
			_spriteRenderer.color = message.MessageColor;
	}


}