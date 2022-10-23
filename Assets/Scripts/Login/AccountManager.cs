using UnityEngine;
using UnityEngine.SceneManagement;

public class AccountManager : MonoBehaviour {

	public static AccountManager Instance { get; private set; }

	[Header("Configuration")]
	[SerializeField] private string mainSceneName = "MainScene";

	public static string Token { get; private set; }
	public static Account Account { get; private set; }
	public static bool IsLogged => Token == null;

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


	public void TryLogin(string username, string password) {
		if(IsLogged) {
			Debug.LogWarning("Tried to log-in... But you're already logged-in !");
			return;
		}
		string hashedPass = HashPassword(password);
		//TODO api call, SetLoginComplete as a callback.
	}

	private void SetLoginComplete(Account account, string token) {
		Account = account;
		Token = token;
		// Go to the main scene.
		SceneManager.LoadScene(mainSceneName);
	}

	private static string HashPassword(string plainPwd) {
		var hash = new Hash128();
		hash.Append(plainPwd);
		//TODO salt in the hash ??
		return hash.ToString();
	}

}