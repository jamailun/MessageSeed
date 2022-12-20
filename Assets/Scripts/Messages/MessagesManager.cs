using UnityEngine;
using UnityEngine.Networking;
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

	private CSharpExtension.Consumable<IEnumerable<Message>> callback;
	public void UpdateMessages(double longitude, double latitude, int zoom, CSharpExtension.Consumable<IEnumerable<Message>> callback = null) {
		if(callback != null)
			this.callback = callback;
		StartCoroutine(CR_UpdateMessagesRequest(longitude, latitude, zoom));
	}

	private IEnumerator CR_UpdateMessagesRequest(double longitude, double latitude, int zoom) {
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

				callback?.Invoke(messages);
			}
		}
	}

	private static string WrapJsonToClass(string source, string topClass) {
		return string.Format("{{ \"{0}\": {1}}}", topClass, source);
	}

	public void WriteMessage(string title, string content, CSharpExtension.Consumable<string> errorCallback, CSharpExtension.Runnable callback) {
		StartCoroutine(CR_SendCreateMessage(title, content, errorCallback, callback));
	}

	private IEnumerator CR_SendCreateMessage(string title, string content, CSharpExtension.Consumable<string> errorCallback, CSharpExtension.Runnable successCallback) {
		string postData = JsonUtility.ToJson(new MessageCreateRequest(title, content));
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
				messages.Add(new Message(new MessageHeader() { author = "4", latitude = GpsPosition.Instance.LastPosition.y, longitude = GpsPosition.Instance.LastPosition.x }));
				callback?.Invoke(messages);
			}
		}
	}

	[System.Serializable]
	private struct MessageCreateRequest {
		public string title;
		public string author;
		public string message;
		public float latitude;
		public float longitude;

		public MessageCreateRequest(string title, string content) {
			this.title = title;
			this.message = content;
			this.author = "4"; // DOBUG
			this.longitude = GpsPosition.Instance.LastPosition.x;
			this.latitude = GpsPosition.Instance.LastPosition.y;
		}
	}

}