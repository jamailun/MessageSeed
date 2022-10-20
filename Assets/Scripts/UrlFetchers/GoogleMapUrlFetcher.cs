using UnityEngine;

public class GoogleMapUrlFetcher : UrlFetcher {

	private readonly uint width, height;
	private readonly string key;

	public GoogleMapUrlFetcher(string key, uint width, uint height) {
		this.key = key;
		this.width = (uint) Mathf.Max(10, width);
		this.height = (uint) Mathf.Max(10, height);
	}

	public string CreateUrl(double latitude, double longitude, int zoom = 10) {
		return
			"http://maps.googleapis.com/maps/api/staticmap?center=" + latitude + "," + longitude +
			"&zoom=" + zoom + "&size=" + width + "x" + height +
			"&key=" + key
			;
	}
	public string CreateUrlTile(int x, int y, int zoom) {
		throw new System.Exception("Cannot get tiles with google api.");
	}

}