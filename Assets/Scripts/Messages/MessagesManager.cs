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

	public void UpdateMessages(double longitude, double latitude, int zoom, CSharpExtension.Consumable<IEnumerable<Message>> callback = null) {
		StartCoroutine(CR_UpdateMessagesRequest(longitude, latitude, zoom, callback));
	}

	private IEnumerator CR_UpdateMessagesRequest(double longitude, double latitude, int zoom, CSharpExtension.Consumable<IEnumerable<Message>> callback) {

		// DEBUG ONLY
		messages.Add(Message.DebugMessage(1));
		messages.Add(Message.DebugMessage(2));
		messages.Add(Message.DebugMessage(3));
		messages[0].RealWorldPosition = GpsPosition.Instance.LastPosition + new Vector2(0.02f, 0.02f);
		messages[1].RealWorldPosition = GpsPosition.Instance.LastPosition - new Vector2(-0.1f, 0.02f);
		messages[2].RealWorldPosition = GpsPosition.Instance.LastPosition + new Vector2(0.05f, 0.01f);

		using(var www = RemoteApiManager.Instance.CreateAuthGetRequest("/messages/")) {
			yield return www.SendWebRequest();
			if(www.result != UnityWebRequest.Result.Success) {
				Debug.LogError(www.error + ". Request feedback: [" + www.downloadHandler?.text+"]");
			} else {
				Debug.Log("success get messages !");
				Debug.Log(www.downloadHandler.text);
				callback?.Invoke(messages);
			}
		}
	}

}