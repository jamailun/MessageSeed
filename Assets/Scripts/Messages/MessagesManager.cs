using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class MessagesManager : MonoBehaviour {

	private readonly List<Message> messages = new();

	public void UpdateMessages(double latitude, double longitude, int zoom) {
		
	}

	public Message CreateNewMessage(string title, string content, int fertilizerAmount) {
		string author = AccountManager.Account.username;

		// Send request
		var msg = new Message(AccountManager.Account, title, content);
		// [msg, fertilizerAmount, POSITION?? ]


		return null;
	}

}