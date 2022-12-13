using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class AccountManager : MonoBehaviour {

	public static AccountManager Instance { get; private set; }

	[Header("Configuration")]
	[SerializeField] private string mainSceneName = "MainScene";

	[Header("Host")]
	[SerializeField] private string _serverUrl = "http://54.38.191.243:8080/";

	public static string Token { get; private set; }
	private static string TokenRefresh { get; set; }
	public static Account Account { get; private set; }
	public static bool IsLogged => Token != null;

	public static bool IsMe(string id) {
		return id == Account.accountId;
	}

	private void Start() {
		// garantie the singleton
		if(Instance != null) {
			Destroy(gameObject);
			return;
		}
		Instance = this;
		// Make persistent accross loading
		DontDestroyOnLoad(gameObject);
	}


	public void TryLogin(string username, string password, CSharpExtension.Consumable<string> errorCallback = null, CSharpExtension.Runnable successCallback = null) {
		if(IsLogged) {
			Debug.LogWarning("Tried to log-in... But you're already logged-in !");
			return;
		}
		Debug.Log("Sending request...");
		StartCoroutine(CR_SendLogin(username, password, errorCallback, successCallback));
	}

	private void SetLoginComplete(LoginResponse response, string username) {
		// Set tokens
		Token = response.access;
		TokenRefresh = response.refresh;
		// fill account data
		Account = new() { accountId = "?", username = username };
		// Go to the main scene.
		SceneManager.LoadScene(mainSceneName);
	}

	private IEnumerator CR_SendLogin(string user, string password, CSharpExtension.Consumable<string> errorCallback, CSharpExtension.Runnable successCallback) {
		string postData = JsonUtility.ToJson(new LoginRequest(user, password));
		byte[] postDataRaw = System.Text.Encoding.UTF8.GetBytes(postData);

		var url = Url("/auth/login/");
		Debug.Log("Sending '" + postData + "' to " + url);

		using(UnityWebRequest www = CreatePostRequest(url, postDataRaw)) {
			yield return www.SendWebRequest();

			if(www.result != UnityWebRequest.Result.Success) {
				Debug.LogError(www.error + " : " + www.downloadHandler?.text);
				errorCallback?.Invoke(www.error + " : " + www.downloadHandler?.text);
			} else {
				var data = JsonUtility.FromJson<LoginResponse>(www.downloadHandler.text);
				SetLoginComplete(data, user);
				successCallback?.Invoke();
			}
		}
	}

	private UnityWebRequest CreatePostRequest(string url, byte[] param) {
		var www = UnityWebRequest.Put(url, param);
		www.method = "POST"; // workaround to easily pass bytes
		www.SetRequestHeader("Content-Type", "application/json");
		//www.chunkedTransfer = false;
		return www;
	}

	private string Url(string suffix) {
		if(_serverUrl.EndsWith('/') && suffix.StartsWith('/'))
			suffix = suffix[1..];
		return _serverUrl + suffix;
	}
}

[System.Serializable]
internal struct LoginResponse {
	public string refresh;
	public string access;
}

[System.Serializable]
internal struct LoginRequest {
	public string username;
	public string password;
	internal LoginRequest(string u, string p) {
		username = u;
		password = p;
	}
}