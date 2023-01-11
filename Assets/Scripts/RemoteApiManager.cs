using UnityEngine;
using UnityEngine.Networking;

public class RemoteApiManager : MonoBehaviour {
	
	public static RemoteApiManager Instance { get; private set; }

	[Header("Host parameters")]
	[SerializeField] private string _serverUrl = "http://54.38.191.243:8080/";

	private void Awake() {
		if(Instance) {
			Destroy(gameObject);
			return;
		}
		Instance = this;
		DontDestroyOnLoad(gameObject);
	}

	public UnityWebRequest CreatePostRequest(string url, bool auth = false) {
		return CreatePostRequest(url, System.Text.Encoding.UTF8.GetBytes(string.Empty), auth);
	}

	public UnityWebRequest CreatePostRequest(string url, byte[] param, bool auth = false) {
		var www = UnityWebRequest.Put(GetUrl(url), param); // create as a Put, but...
		www.method = "POST"; //... set to POST. This is a workaround to easily pass bytes to a POST.
		www.SetRequestHeader("Content-Type", "application/json");
		if(auth)
			return AuthenticateRequest(www);
		return www;
	}

	public UnityWebRequest CreateAuthGetRequest(string url) {
		return AuthenticateRequest(UnityWebRequest.Get(GetUrl(url)));
	}
	public UnityWebRequest CreateAuthPutRequest(string url) {
		var www = UnityWebRequest.Get(GetUrl(url));
		www.method = "PUT";
		return AuthenticateRequest(www);
	}

	public UnityWebRequest AuthenticateRequest(UnityWebRequest request) {
		if(!AccountManager.IsLogged) {
			Debug.LogError("Could not authenticate request. AccountManager is NOT logged-in.");
			return request;
		}
		request.SetRequestHeader("Authorization", "Bearer " + AccountManager.Token);
		return request;
	}

	public string GetUrl(string suffix) {
		// avoid a '//' problem.
		if(_serverUrl.EndsWith('/') && suffix.StartsWith('/'))
			suffix = suffix[1..];
		// add a '/' at the end
		if(!suffix.EndsWith('/'))
			suffix += '/';
		return _serverUrl + suffix;
	}


}