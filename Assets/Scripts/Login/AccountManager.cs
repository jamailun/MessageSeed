using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class AccountManager : MonoBehaviour {

	public static AccountManager Instance { get; private set; }

	[Header("Configuration")]
	[SerializeField] private string loginSceneName = "LoginScene";
	[SerializeField] private string mainSceneName = "MainScene";

	public static string Token => Account.tokenAccess;
	private static string TokenRefresh => Account.tokenRefresh;
	public static Account Account { get; private set; }
	public static bool IsLogged => Account != null && Token != null;

	public static bool IsMe(string id) {
		return Account != null && id == Account.accountId;
	}

	private void Awake() {
		// garantie the singleton
		if(Instance != null) {
			Destroy(gameObject);
			return;
		}
		Instance = this;
		// Make persistent accross loading
		DontDestroyOnLoad(gameObject);
	}

	private void Start() {
		// try log-in directly
		if(LocalData.HasAccount()) {
			Account = LocalData.GetAccount();
			Debug.Log("Data found. Username is '" + Account.username + "'.");
			SceneManager.LoadScene(mainSceneName);
		}
	}

	public void InvalidateSession() {
		Debug.LogWarning("Session invalidated.");
		Account = null;
	}


	public void TryLogin(string username, string password, CSharpExtension.Consumable<string> errorCallback = null, CSharpExtension.Runnable successCallback = null) {
		if(IsLogged) {
			Debug.LogWarning("Tried to log-in... But you're already logged-in !");
			return;
		}
		StartCoroutine(CR_SendLogin(username, password, errorCallback, successCallback));
	}

	public void TryLogout() {
		if(!IsLogged) {
			Debug.LogWarning("Tried to log-out... But you're NOT logged-in !");
			return;
		}
		StartCoroutine(CR_SendLogout());
	}

	private void SetLoginComplete(LoginResponse response, string username) {
		// fill account data
		Account = new() { accountId = "?", username = username, tokenAccess = response.access, tokenRefresh = response.refresh };
		LocalData.SaveAccount(Account);
		Debug.Log("Saved local data.");
		// Go to the main scene.
		SceneManager.LoadScene(mainSceneName);
	}

	private IEnumerator CR_SendLogin(string user, string password, CSharpExtension.Consumable<string> errorCallback, CSharpExtension.Runnable successCallback) {
		string postData = JsonUtility.ToJson(new LoginRequest(user, password));
		byte[] postDataRaw = System.Text.Encoding.UTF8.GetBytes(postData);

		using(UnityWebRequest www = RemoteApiManager.Instance.CreatePostRequest("/api_auth/login/", postDataRaw)) {
			yield return www.SendWebRequest();

			if(www.result != UnityWebRequest.Result.Success) {
				Debug.LogError(www.error + " : " + www.downloadHandler?.text);
				errorCallback?.Invoke(www.error + " : " + www.downloadHandler?.text);
			} else {
				var data = JsonUtility.FromJson<LoginResponse>(www.downloadHandler.text);
				SetLoginComplete(data, user);
				Debug.Log("LOGIN COMPLETED !");
				successCallback?.Invoke();
			}
		}
	}
	private IEnumerator CR_SendLogout() {
		string postData = JsonUtility.ToJson(new LogoutRequest(TokenRefresh));
		byte[] postDataRaw = System.Text.Encoding.UTF8.GetBytes(postData);
		using(UnityWebRequest www = RemoteApiManager.Instance.CreatePostRequest("/api_auth/logout/", postDataRaw, true)) {
			yield return www.SendWebRequest();

			if(www.result != UnityWebRequest.Result.Success) {
				Debug.LogError(www.error + " : " + www.downloadHandler?.text);
			} else {
				Debug.Log("LOGOUT COMPLETED !");
				LocalData.ClearAccount();
				Account = null;
				SceneManager.LoadScene(loginSceneName);
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
[System.Serializable]
internal struct LogoutRequest {
	public string refresh_token;
	internal LogoutRequest(string refresh_token) {
		this.refresh_token = refresh_token;
	}
}