using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class MessageRenderer : MonoBehaviour {

// DEBUUUG !!!
	[SerializeField] private Message _message;
	private bool IsLoaded => _message.IsComplete;

	public Message Message => _message;

	public void SetMessage(Message message) {
		this._message = message;
	}

	public void OpenOrLoad(CSharpExtension.Consumable<Message> callback) {
		if(Message == null) {
			Debug.LogError("ERRROR tried to load messageRenderer " + name + "... But NO message has been provided.");
			return;
		}
		if(IsLoaded) {
			Debug.Log("message already loaded...");
			callback?.Invoke(Message);
			return;
		}
		StartCoroutine(CR_GetMessageDetails(callback));
	}

	private IEnumerator CR_GetMessageDetails(CSharpExtension.Consumable<Message> callback) {
		using(var www = RemoteApiManager.Instance.CreateAuthGetRequest("/messages/"+Message.MessageId+"/")) {
			yield return www.SendWebRequest();
			if(www.result != UnityWebRequest.Result.Success) {
				Debug.LogError(www.error + " : " + www.downloadHandler?.text);
			} else {
				var messageComplete = JsonUtility.FromJson<MessageComplete>(www.downloadHandler.text);
				Message.Complete(messageComplete);
				Debug.Log("Got message complete of " + Message.MessageId + " successfully:" + messageComplete);
				callback?.Invoke(Message);
			}
		}
	}


}