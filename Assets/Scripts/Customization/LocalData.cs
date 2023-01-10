using UnityEngine;

public static class LocalData {

	private const string PREFIX = "jp.messageseed.";

	public static bool HasAccount() {
		return PlayerPrefs.HasKey(PREFIX + "username")
			&& PlayerPrefs.HasKey(PREFIX + "token.access")
			&& PlayerPrefs.HasKey(PREFIX + "token.refresh");
	}

	public static Account GetAccount() {
		return new Account() {
			username = PlayerPrefs.GetString(PREFIX + "username"),
			tokenAccess = PlayerPrefs.GetString(PREFIX + "token.access"),
			tokenRefresh = PlayerPrefs.GetString(PREFIX + "token.refresh")
		};
	}

	public static void SaveAccount(Account account) {
		PlayerPrefs.SetString(PREFIX + "username", account.username);
		PlayerPrefs.SetString(PREFIX + "token.access", account.tokenAccess);
		PlayerPrefs.SetString(PREFIX + "token.refresh", account.tokenRefresh);
		PlayerPrefs.Save();
	}

	public static PlayerPreferences GetPreferences() {
		return new PlayerPreferences() {
			modelVariantName = PlayerPrefs.GetString(PREFIX + "prefs.model")
		};
	}

	public static void SavePreferences(PlayerPreferences prefs) {
		PlayerPrefs.SetString(PREFIX + "prefs.model", prefs.modelVariantName);
		PlayerPrefs.Save();
	}

	public struct PlayerPreferences {
		public string modelVariantName;
	}
	
}