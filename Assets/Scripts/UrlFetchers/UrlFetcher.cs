using UnityEngine;

public interface UrlFetcher {

	public string CreateUrl(double latitude, double longitude, int zoom = 10);

	public string CreateUrlTile(int x, int y, int zoom);

}