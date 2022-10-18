using UnityEditor;
using UnityEngine;

public class GoogleMapUrlFetcher : UrlFetcher {

	private uint width, height;
	private string key;

	public GoogleMapUrlFetcher(string key, uint width, uint height) {
		this.key = key;
		this.width = (uint) Mathf.Max(10, width);
		this.height = (uint) Mathf.Max(10, height);
	}

	public string CreateUrl(Vector2 position, int zoom = 10) {
		return
			"http://maps.googleapis.com/maps/api/staticmap?center=" + Serialize(position) +
			"&zoom=" + zoom + "&size=" + width + "x" + height +
			"&key=" + key
			;
	}

	private static string Serialize(Vector2 center) {
		return center.x + "," + center.y;
	}

}