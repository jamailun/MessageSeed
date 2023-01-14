using UnityEngine;

public static class LocalData {

	private const string PREFIX = "jp.messageseed.";

	private const string USERNAME = PREFIX + "username";
	private const string TOKEN_ACCESS = PREFIX + "token.access";
	private const string TOKEN_REFRESH = PREFIX + "token.refresh";

	private const string PREFS_MODEL = PREFIX + "prefs.model";
	private const string PREFS_SKY = PREFIX + "prefs.sky";
	private const string PREFS_SIZES_AVATAR = PREFIX + "prefs.sizes.avatar";
	private const string PREFS_SIZES_MESSAGES = PREFIX + "prefs.sizes.messages";
	private const string PREFS_SIZES_BUFFER = PREFIX + "prefs.sizes.buffer";

	public static bool HasAccount() {
		return PlayerPrefs.HasKey(USERNAME)
			&& PlayerPrefs.HasKey(TOKEN_ACCESS)
			&& PlayerPrefs.HasKey(TOKEN_REFRESH);
	}

	public static Account GetAccount() {
		return new Account() {
			username = PlayerPrefs.GetString(USERNAME),
			tokenAccess = PlayerPrefs.GetString(TOKEN_ACCESS),
			tokenRefresh = PlayerPrefs.GetString(TOKEN_REFRESH)
		};
	}

	public static void SaveAccount(Account account) {
		PlayerPrefs.SetString(USERNAME, account.username);
		PlayerPrefs.SetString(TOKEN_ACCESS, account.tokenAccess);
		PlayerPrefs.SetString(TOKEN_REFRESH, account.tokenRefresh);
		PlayerPrefs.Save();
	}

	public static void ClearAccount() {
		PlayerPrefs.DeleteKey(USERNAME);
		PlayerPrefs.DeleteKey(TOKEN_ACCESS);
		PlayerPrefs.DeleteKey(TOKEN_REFRESH);
		PlayerPrefs.Save();
	}

	public static string GetPreferredModel() {
		return PlayerPrefs.GetString(PREFS_MODEL);
	}
	public static string GetPreferredSky() {
		return PlayerPrefs.GetString(PREFS_SKY);
	}

	public static void SavePreferredModel(VariantModel model) {
		PlayerPrefs.SetString(PREFS_MODEL, model.VariantName);
		PlayerPrefs.Save();
	}
	public static void SavePreferredSky(VariantSky model) {
		PlayerPrefs.SetString(PREFS_SKY, model.VariantName);
		PlayerPrefs.Save();
	}

	public static void SaveSizes(float avatar, float messages, int buffer) {
		PlayerPrefs.SetFloat(PREFS_SIZES_AVATAR, avatar);
		PlayerPrefs.SetFloat(PREFS_SIZES_MESSAGES, messages);
		PlayerPrefs.SetInt(PREFS_SIZES_BUFFER, buffer);
		PlayerPrefs.Save();
	}

	public static float GetSizeAvatar() {
		return PlayerPrefs.GetFloat(PREFS_SIZES_AVATAR, 1f);
	}
	public static float GetSizeMessages() {
		return PlayerPrefs.GetFloat(PREFS_SIZES_MESSAGES, 1f);
	}
	public static int GetSizeBuffer() {
		return PlayerPrefs.GetInt(PREFS_SIZES_BUFFER, 1);
	}

}