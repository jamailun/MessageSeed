using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class TestJson : MonoBehaviour {

	public string token;
	public string json;

	public void truc() {
		StartCoroutine(AAAAA());
	}

	private IEnumerator AAAAA() {

		var www = UnityWebRequest.Get("http://54.38.191.243:8080/messages/");
		www.SetRequestHeader("Authorization", "Bearer " + token);

		yield return www.SendWebRequest();

		if(www.result != UnityWebRequest.Result.Success) {
			Debug.LogError(www.error + ". URL=[" + www.url + "] Request feedback: [" + www.downloadHandler?.text + "]");
		} else {
			string json = www.downloadHandler.text;
			Debug.Log("success get messages : " + json);

			var d = JsonUtility.FromJson<MessageHeader[]>(json);
			Debug.Log("=> " + d);
		}
	}

	public void machin() {
		string json2 = WrapJsonToClass(json, "list");
		var d = JsonUtility.FromJson<MessagesHeaderList>(json2);
		Debug.Log("=> " + d);
		foreach(var h in d.list) {
			Debug.Log("--> " + h);
		}
	}

	public static string WrapJsonToClass(string source, string topClass) {
		return string.Format("{{ \"{0}\": {1}}}", topClass, source);
	}
	
	[System.Serializable]
	public struct MessagesHeaderList {
		public MessageHeader[] list;
	}

}