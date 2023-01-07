using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Collections;

using GoMap;
using GoShared;

public class MessagesManager : MonoBehaviour {

	public static MessagesManager Instance { get; private set; }
	[SerializeField] private bool debugData = false;

	[SerializeField] private NewMessagesEvent newMessagesEvent;

	private readonly List<Message> messages = new();
	private Coordinates lastCoordinates;

	private void Awake() {
		if(Instance) {
			Destroy(gameObject);
			return;
		}
		Instance = this;
		DontDestroyOnLoad(gameObject);
	}

	private bool callingServer = false;
	public void UpdateMessages(Coordinates coordinates) {
		lastCoordinates = coordinates;
		if(callingServer)
			return;
		callingServer = true;
		if(debugData) {
			if(messages.Count == 0) {
				messages.Add(Message.DebugMessage(1, coordinates));
				messages.Add(Message.DebugMessage(2, coordinates));
				messages.Add(Message.DebugMessage(3, coordinates));
				messages.Add(Message.DebugMessage(4, coordinates));
			}
			newMessagesEvent?.Invoke(messages);
			callingServer = false;
			return;
		}
		Debug.LogWarning("start calling sever to getmesasges.");
		// Normal
		StartCoroutine(CR_UpdateMessagesRequest());
	}

	private IEnumerator CR_UpdateMessagesRequest() {
		using(var www = RemoteApiManager.Instance.CreateAuthGetRequest("/messages/")) {
			yield return www.SendWebRequest();
			if(www.result != UnityWebRequest.Result.Success) {
				Debug.LogError(www.error + ". URL=["+www.url+"] Request feedback: [" + www.downloadHandler?.text+"]");
			} else {
				string json = www.downloadHandler.text;
				Debug.Log("success get messages : " + json);

				// Unity CANNOT handle [{}...]. It needs to be wrapped as {list:[{}...]}
				string jsonWrapped = WrapJsonToClass(json, "list");
				var headers = JsonUtility.FromJson<MessagesHeaderList>(jsonWrapped);

				foreach(var header in headers.list) {
					messages.Add(new Message(header));
				}

				newMessagesEvent?.Invoke(messages);
			}
			callingServer = false;
		}
	}

	private static string WrapJsonToClass(string source, string topClass) {
		return string.Format("{{ \"{0}\": {1}}}", topClass, source);
	}

	public void WriteMessage(string title, string content, CSharpExtension.Consumable<string> errorCallback, CSharpExtension.Runnable callback) {
		StartCoroutine(CR_SendCreateMessage(title, content, errorCallback, callback));
	}

	private IEnumerator CR_SendCreateMessage(string title, string content, CSharpExtension.Consumable<string> errorCallback, CSharpExtension.Runnable successCallback) {
		var dataSent = new MessageCreateRequest(title, content, lastCoordinates);
		string postData = JsonUtility.ToJson(dataSent);
		byte[] postDataRaw = System.Text.Encoding.UTF8.GetBytes(postData);

		using(UnityWebRequest www = RemoteApiManager.Instance.CreatePostRequest("/database/message/create/", postDataRaw, true)) {
			yield return www.SendWebRequest();

			if(www.result != UnityWebRequest.Result.Success) {
				Debug.LogError(www.error + " : " + www.downloadHandler?.text);
				errorCallback?.Invoke(www.error + " : " + www.downloadHandler?.text);
			} else {
				Debug.Log("new message posted successfully !");
				var data = JsonUtility.FromJson<LoginResponse>(www.downloadHandler.text);
				successCallback?.Invoke();
				Message msg = new(new MessageHeader() { author = AccountManager.Account.accountId, latitude = dataSent.latitude, longitude = dataSent.longitude });
				messages.Add(msg);
				List<Message> lm = new();
				lm.Add(msg);
				newMessagesEvent?.Invoke(lm);
			}
		}
	}

	[System.Serializable]
	private struct MessageCreateRequest {
		public string title;
		public string message;
		public double latitude;
		public double longitude;

		public MessageCreateRequest(string title, string content, Coordinates coordinates) {
			this.title = title;
			this.message = content;
			this.longitude = coordinates.longitude;
			this.latitude = coordinates.latitude;
		}
	}


	[System.Serializable]
	public class NewMessagesEvent : UnityEngine.Events.UnityEvent<List<Message>> { }

}