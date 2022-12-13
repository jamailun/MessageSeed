using UnityEngine;

public interface UrlFetcher {

	/// <deprecated>
	/// use #CreateUrlTile
	/// </deprecated>
	[System.Obsolete("Use CreateUrlTile instead", true)]
	public string CreateUrl(double latitude, double longitude, int zoom = 10);

	public string CreateUrlTile(int x, int y, int zoom);

}