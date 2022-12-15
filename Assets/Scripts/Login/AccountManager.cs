using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class AccountManager : MonoBehaviour {

	public static AccountManager Instance { get; private set; }

	[Header("Configuration")]
	[SerializeField] private string mainSceneName = "MainScene";

	public static string Token { get; private set; }
	private static string TokenRefresh { get; set; }
	public static Account Account { get; private set; }
	public static bool IsLogged => Token != null;

	public static bool IsMe(string id) {
		return Account != null && id == Account.accountId;
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

		var url = RemoteApiManager.Instance.GetUrl("/auth/login/");
		Debug.Log("Sending '" + postData + "' to " + url);

		using(UnityWebRequest www = RemoteApiManager.Instance.CreatePostRequest(url, postDataRaw)) {
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