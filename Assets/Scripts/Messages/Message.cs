using UnityEngine;

/// <summary>
/// Serialized from JSON.
/// </summary>
[System.Serializable]
public class Message {

	public long messageId; // Comme discord, le timestamp == le message id

	public string messageTitle;

	public string messageContent;

	public string authorName;

	public string authorId;

	public long deathTime; // timecode of the death

	public long likesAmount;

	public float longitude, latitude;

	public Color MessageColor => AccountManager.IsMe(authorId) ? Color.yellow : Color.blue;

	public Message() { /* JSON constructor */ }

	public Message(Account author, string title, string content) {
		messageTitle = title;
		messageContent = content;
		authorName = author.username;
		authorId = author.accountId;
	}

	public Vector2 RealWorldPosition {
		set { longitude = value.x; latitude = value.y; }
		get { return new(longitude, latitude); }
	}

	public bool ExistsOnServer => messageId != 0;

	public const float EARTH_RADIUS_KM = 6371f;
	// Code adapted from https://www.geeksforgeeks.org/program-distance-two-points-earth/
	public float GetDistanceRealWorld(Vector2 realWorldPosition) {
		float lon1 = realWorldPosition.x * Mathf.Deg2Rad;
		float lon2 = longitude * Mathf.Deg2Rad;
		float lat1 = realWorldPosition.y * Mathf.Deg2Rad;
		float lat2 = latitude * Mathf.Deg2Rad;

		// Haversine formula
		float dlon = lon2 - lon1;
		float dlat = lat2 - lat1;
		float a = Mathf.Pow(Mathf.Sin(dlat / 2), 2) + Mathf.Cos(lat1) * Mathf.Cos(lat2) * Mathf.Pow(Mathf.Sin(dlon / 2), 2);
		float c = 2 * Mathf.Asin(Mathf.Sqrt(a));

		return c * EARTH_RADIUS_KM;
	}

	public static Message DebugMessage(int i) {
		return new Message(new Account("test_" + i, "test_" + i), "TEST_" + i, "Message de test n°" + i + ".");
	}

}