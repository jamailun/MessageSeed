using UnityEngine;
using System;

public class GeoApiFyUrlFetcher : UrlFetcher {

	private readonly string token;

	public GeoApiFyUrlFetcher(string token) {
		this.token = token;
	}

	public string CreateUrl(Vector2 position, int zoom = 10) {
		int x = long2tilex(position.x, zoom);
		int y = lat2tiley(position.y, zoom);
		Debug.Log("Fecthing image [" + zoom + "/" + x + "/" + y + "]");
		return "https://maps.geoapify.com/v1/tile/carto/" + zoom + "/" + x + "/" + y + ".png?&apiKey=" + token;
	}

	private int long2tilex(double lon, int z) {
		return (int) (Math.Floor((lon + 180.0) / 360.0 * (1 << z)));
	}
	private int lat2tiley(double lat, int z) {
		return (int) Math.Floor((1 - Math.Log(Math.Tan(ToRadians(lat)) + 1 / Math.Cos(ToRadians(lat))) / Math.PI) / 2 * (1 << z));
	}
	public double ToRadians(double angle) {
		return (Math.PI / 180) * angle;
	}

}