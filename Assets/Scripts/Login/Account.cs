/// <summary>
/// Serialized from JSON.
/// </summary>
[System.Serializable]
public class Account {

	public string accountId;

	public string username;

	public Account() { }

	public Account(string id, string name) {
		this.accountId = id;
		this.username = name;
	}

}