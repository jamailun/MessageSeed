using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class MessageRenderer : MonoBehaviour {

	[Header("Displays")]
	[SerializeField] private GameObject prefab_seed;
	[SerializeField] private GameObject prefab_bush;
	[SerializeField] private GameObject prefab_tree;

	[Header("Configuration")]
	[SerializeField] private int startBush = 10;
	[SerializeField] private int startTree = 50;

	private int LikesAmount => Message == null ? 0 : (int) Message.likesAmount;
	private bool IsLoaded => Message != null && Message.IsComplete;
	public Message Message { get; private set; }

	public void SetMessage(Message message) {
		Message = message;
		UpdateChild();
	}

	private void UpdateChild() {
		transform.DestroyChildren();
		var prefab = LikesAmount >= startTree ? prefab_tree : LikesAmount >= startBush ? prefab_bush : prefab_seed;
		var child = Instantiate(prefab, transform);
		child.transform.localPosition = Vector3.zero;
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
		using(var www = RemoteApiManager.Instance.CreateAuthGetRequest("/database/message/" + Message.MessageId+"/")) {
			yield return www.SendWebRequest();
			if(www.result != UnityWebRequest.Result.Success) {
				Debug.LogError(www.error + " : " + www.downloadHandler?.text);
			} else {
				var messageComplete = JsonUtility.FromJson<MessageComplete>(www.downloadHandler.text);
				Message.Complete(messageComplete);
				Debug.Log("Got message complete of " + Message.MessageId + " successfully:" + messageComplete+"\n"+www.downloadHandler.text);
				callback?.Invoke(Message);
				UpdateChild();
			}
		}
	}


}