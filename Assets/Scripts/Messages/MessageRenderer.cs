using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class MessageRenderer : MonoBehaviour {

	[Header("Displays")]
	[SerializeField] private GameObject prefab_seed;
	[SerializeField] private GameObject prefab_bush;
	[SerializeField] private GameObject prefab_tree;
	[Header("Values")]
	[SerializeField] private float randomAmplitude = 0.5f;
	[SerializeField] private float bonusHeight = 0.5f;
	[SerializeField] private float likeScaleBonus = 0.01f;

	private bool IsLoaded => Message != null && Message.IsComplete;
	public Message Message { get; private set; }

	public void SetMessage(Message message) {
		Message = message;
		UpdateChild();
	}

	public void RandomizePosition() {
		// set random rotation
		transform.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
		// and random position delta
		transform.localPosition += new Vector3(
			Random.Range(-randomAmplitude, randomAmplitude),
			bonusHeight,
			Random.Range(-randomAmplitude, randomAmplitude)
		);
	}

	private void UpdateChild() {
		transform.DestroyChildren();
		var prefab = Message.State switch {
			MessageState.Seed => prefab_seed,
			MessageState.Sapling => prefab_bush,
			MessageState.Tree => prefab_tree,
			_ => null
		};
		if(prefab == null) {
			Debug.LogError("Unexpected State for message " + Message + " : " + Message.State);
			return;
		}
		var child = Instantiate(prefab, transform);
		child.transform.localScale = (1f + Message.LikesAmount * likeScaleBonus) * Vector3.one;
	}

	public void OpenOrLoad(CSharpExtension.Consumable<Message> callback) {
		if(Message == null) {
			Debug.LogError("ERRROR tried to load messageRenderer " + name + "... But NO message has been provided.");
			return;
		}
		// WE DO IT ANYWAY
		StartCoroutine(CR_GetMessageDetails(callback));
		if(IsLoaded) {
			callback?.Invoke(Message);
			return;
		}
	}

	private IEnumerator CR_GetMessageDetails(CSharpExtension.Consumable<Message> callback) {
		using(var www = RemoteApiManager.Instance.CreateAuthGetRequest("/database/message/" + Message.MessageId+"/")) {
			yield return www.SendWebRequest();
			if(www.result != UnityWebRequest.Result.Success) {
				Debug.LogError(www.error + " : " + www.downloadHandler?.text);
			} else {
				var messageComplete = JsonUtility.FromJson<MessageComplete>(www.downloadHandler.text);
				Message.Complete(messageComplete);
				UpdateChild();
				callback?.Invoke(Message);
			}
		}
	}


}