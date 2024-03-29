﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class AccountManager : MonoBehaviour {

	public static AccountManager Instance { get; private set; }

	[Header("Configuration")]
	[SerializeField] private string loginSceneName = "LoginScene";
	[SerializeField] private string mainSceneName = "MainScene";
	[SerializeField] private ServerUnreachableUI unreachableUi;

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
			//TODO display loading!
			var account = LocalData.GetAccount();
			Debug.Log("Data found. Username is '" + account.username + "'. Start validity test.");
			TryGetProfile(
				p => {
					// profile obtained successfully. It means token is valid.
					Debug.Log("Success : loading main scene");
					Account = account;
					SceneManager.LoadScene(mainSceneName);
				}, code => {
					Debug.LogWarning("Token invalid : " + code);
					// token invalid ? so it's ok, we do nothing.
				},
				account
			);
		}
	}

	public void InvalidateSession() {
		Debug.LogWarning("Session invalidated.");
		Account = null;
	}

	public void TryLogin(string username, string password, CSharpExtension.Consumable<string> errorCallback = null) {
		if(IsLogged) {
			Debug.LogWarning("Tried to log-in... But you're already logged-in !");
			errorCallback?.Invoke("Cannot log-in : already logged-in !");
			return;
		}
		StartCoroutine(CR_SendLogin(username, password, errorCallback));
	}

	public void TrySignIn(string username, string mail, string password, CSharpExtension.Consumable<string> errorCallback = null) {
		if(IsLogged) {
			Debug.LogWarning("Tried to sign-in... But you're already logged-in !");
			errorCallback?.Invoke("Cannot sign-in : already logged-in !");
			return;
		}
		StartCoroutine(CR_SendSignIn(username, mail, password, errorCallback));
	}

	public void TryGetProfile(CSharpExtension.Consumable<ProfileResponse> success, CSharpExtension.Consumable<int> error, Account differenceAccount = null) {
		StartCoroutine(CR_GetProfile(success, error, differenceAccount));
	}

	public void TryLogout() {
		if(!IsLogged) {
			Debug.LogWarning("Tried to log-out... But you're NOT logged-in !");
			return;
		}
		StartCoroutine(CR_SendLogout());
	}

	public void TryGetMyMessagesList(CSharpExtension.Consumable<IEnumerable<MessageListSerializer>> success) {
		StartCoroutine(CR_GetMyMessagesList(success));
	}

	private void SetLoginComplete(LoginResponse response, string username) {
		// fill account data
		Account = new() { accountId = "?", username = username, tokenAccess = response.access, tokenRefresh = response.refresh };
		LocalData.SaveAccount(Account);
		Debug.Log("Saved local data.");
		// Go to the main scene.
		SceneManager.LoadScene(mainSceneName);
	}

	private IEnumerator CR_SendLogin(string user, string password, CSharpExtension.Consumable<string> errorCallback) {
		string postData = JsonUtility.ToJson(new LoginRequest(user, password));
		byte[] postDataRaw = System.Text.Encoding.UTF8.GetBytes(postData);

		using(UnityWebRequest www = RemoteApiManager.Instance.CreatePostRequest("/api_auth/login/", postDataRaw)) {
			yield return www.SendWebRequest();

			if(www.result != UnityWebRequest.Result.Success) {
				Debug.LogError(www.error + " : " + www.downloadHandler?.text);
				errorCallback?.Invoke(www.error + " : " + www.downloadHandler?.text);
			} else {
				var data = JsonUtility.FromJson<LoginResponse>(www.downloadHandler.text);
				Debug.Log("LOGIN COMPLETED !");
				SetLoginComplete(data, user);
			}
		}
	}

	private IEnumerator CR_SendSignIn(string user, string mail, string password, CSharpExtension.Consumable<string> errorCallback) {
		string postData = JsonUtility.ToJson(new SignInRequest(user, mail, password));
		byte[] postDataRaw = System.Text.Encoding.UTF8.GetBytes(postData);
		using(UnityWebRequest www = RemoteApiManager.Instance.CreatePostRequest("/api_auth/register/", postDataRaw)) {
			yield return www.SendWebRequest();

			if(www.result != UnityWebRequest.Result.Success) {
				Debug.LogError(www.error + " : " + www.downloadHandler?.text);
				errorCallback?.Invoke(www.error + " : " + www.downloadHandler?.text);
			} else {
				var data = JsonUtility.FromJson<SignInResponse>(www.downloadHandler.text);
				Debug.Log("SIGNGIN COMPLETED ! Now sending log-in...");
				StartCoroutine(CR_SendLogin(user, password, errorCallback));
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

	private IEnumerator CR_GetProfile(CSharpExtension.Consumable<ProfileResponse> success, CSharpExtension.Consumable<int> error, Account differentAccount = null) {
		differentAccount ??= Account;
		using(UnityWebRequest www = RemoteApiManager.Instance.CreateAuthGetRequest("/database/profile/", differentAccount.tokenAccess)) {
			yield return www.SendWebRequest();

			if(www.result != UnityWebRequest.Result.Success) {
				Debug.LogError("[GETTING PROFILE] ("+www.responseCode+")" + www.error + " : " + www.downloadHandler?.text);
				error?.Invoke((int)www.responseCode);
			} else {
				var profile = JsonUtility.FromJson<ProfileResponse>(www.downloadHandler.text);
				differentAccount.accountId = profile.user_id;
				success?.Invoke(profile);
			}
		}
	}

	private IEnumerator CR_GetMyMessagesList(CSharpExtension.Consumable<IEnumerable<MessageListSerializer>> success) {
		using(UnityWebRequest www = RemoteApiManager.Instance.CreateAuthGetRequest("/database/profile/messages/")) {
			yield return www.SendWebRequest();

			if(www.result != UnityWebRequest.Result.Success) {
				Debug.LogError("[GETTING MSG LIST] (" + www.responseCode + ")" + www.error + " : " + www.downloadHandler?.text);
			} else {
				Debug.LogWarning("list of MY messages : " + www.downloadHandler.text);
				var list = JsonUtility.FromJson<MessageListSerializerList>(www.downloadHandler.text);
				success?.Invoke(list.my_messages);
			}
		}
	}
}

[System.Serializable]
public struct ProfileResponse {
	public string user_id;
	public string author_name;
	// exp
	public double level;
	public double experience;
	public double experience_next;
	public double experience_previous;
	// stats
	public int likes_received_total;
	public int likes_given_total;
	public int most_liked_message_count;
	public string most_liked_message_id;
}

[System.Serializable]
internal struct LoginResponse {
	public string refresh;
	public string access;
}

[System.Serializable]
internal struct SignInResponse {
	public string username;
	public string email;
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
internal struct SignInRequest {
	public string username;
	public string password;
	public string password2;
	public string email;
	public string first_name;
	public string last_name;
	internal SignInRequest(string u, string m, string p) {
		username = u;
		password = p;
		password2 = p;
		email = m;
		first_name = u;
		last_name = "-";
	}
}

[System.Serializable]
internal struct LogoutRequest {
	public string refresh_token;
	internal LogoutRequest(string refresh_token) {
		this.refresh_token = refresh_token;
	}
}
