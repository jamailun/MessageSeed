using UnityEngine;
using UnityEngine.Networking;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

public class MessagesManager : MonoBehaviour {

	public static MessagesManager Instance { get; private set; }

	private readonly List<Message> messages = new();

	private void Awake() {
		if(Instance) {
			Destroy(gameObject);
			return;
		}
		Instance = this;
		DontDestroyOnLoad(gameObject);
	}

	public void UpdateMessages(double longitude, double latitude, int zoom) {
		StartCoroutine(CR_UpdateMessagesRequest(longitude, latitude, zoom));
	}

	private IEnumerator CR_UpdateMessagesRequest(double longitude, double latitude, int zoom) {
		using(var www = RemoteApiManager.Instance.CreateGetRequest("/tiles/")) {
			yield return www.SendWebRequest();
			if(www.result != UnityWebRequest.Result.Success) {
				Debug.LogError(www.error + " : " + www.downloadHandler?.text);
			} else {
				Debug.Log("success get tiles !");
			}
		}
	}

	public Message CreateNewMessage(string title, string content, int fertilizerAmount) {
		string author = AccountManager.Account.username;

		// Send request
		var msg = new Message(AccountManager.Account, title, content);
		// [msg, fertilizerAmount, POSITION?? ]


		return null;
	}

}