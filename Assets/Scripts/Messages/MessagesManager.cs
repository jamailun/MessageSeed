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
		yield return new WaitForEndOfFrame();

		messages.Add(Message.DebugMessage(1));
		messages.Add(Message.DebugMessage(2));
		messages.Add(Message.DebugMessage(3));
		messages[0].Position = GpsPosition.Instance.LastPosition + new Vector2(0.02f, 0.02f);
		messages[1].Position = GpsPosition.Instance.LastPosition - new Vector2(0.02f, 0.02f);
		messages[2].Position = GpsPosition.Instance.LastPosition + new Vector2(0.05f, 0.01f);

		/*using(var www = RemoteApiManager.Instance.CreateGetRequest("/messages/")) {
			yield return www.SendWebRequest();
			if(www.result != UnityWebRequest.Result.Success) {
				Debug.LogError(www.error + " : " + www.downloadHandler?.text);
			} else {
				Debug.Log("success get tiles !");
			}
		}*/

		callback?.Invoke(messages);
	}

	public Message CreateNewMessage(string title, string content, int fertilizerAmount) {
		// Send request
		var msg = new Message(AccountManager.Account, title, content) {
			Position = GpsPosition.Instance.LastPosition
		};
		// fertilizerAmount ??


		return msg;
	}

}