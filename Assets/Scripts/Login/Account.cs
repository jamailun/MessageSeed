/// <summary>
/// Serialized from JSON.
/// </summary>
[System.Serializable]
public class Account {

	public string accountId; //TO DETERMINE

	public string username;

	public string tokenAccess;
	public string tokenRefresh;

	public Account() { }

	public Account(string id, string name) {
		this.accountId = id;
		this.username = name;
	}

}