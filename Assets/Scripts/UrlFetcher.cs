using UnityEngine;

public interface UrlFetcher {

	/// <summary>
	/// Create an URL from the parameters.
	/// </summary>
	/// <param name="position">The position of the center of the map.</param>
	/// <param name="zoom">The current zoom value.</param>
	/// <returns>An url to resquest to.</returns>
	public string CreateUrl(Vector2 position, int zoom = 10);

	protected string SerializePosition(Vector2 position) {
		return position.x + "," + position.y;
	}

}