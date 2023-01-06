using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class MessageRenderer : MonoBehaviour {

	private Message _message;
	private bool IsLoaded => _message != null && _message.IsComplete;

	public GoShared.Coordinates coordinates;

	public Message Message => _message;

	public void SetMessage(Message message) {
		this._message = message;
		coordinates = message.Coordinates;
		UpdateColor();
	}

	private void UpdateColor() {
		//TODO !
	}

	public void OpenOrLoad(CSharpExtension.Consumable<Message> callback) {
		if(Message == null) {
			Debug.LogError("ERRROR tried to load messageRenderer " + name + "... But NO message has been provided.");
			return;
		}
		if(IsLoaded) {
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
				Debug.Log("success get messages !");
				Debug.Log(www.downloadHandler.text);
				var messageComplete = JsonUtility.FromJson<MessageComplete>(www.downloadHandler.text);
				// apply
				Message.Complete(messageComplete);
				UpdateColor();

				callback?.Invoke(Message);
			}
		}
	}


}