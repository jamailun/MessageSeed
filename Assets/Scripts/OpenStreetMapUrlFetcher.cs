using UnityEngine;
using System;

public class OpenStreetMapURL : UrlFetcher {

	private readonly string[] SERVER = {"a", "b", "c"};

	public OpenStreetMapURL() {}

	public string CreateUrl(Vector2 position, int zoom = 10) {
		string server = SERVER[UnityEngine.Random.Range(0, 2)];
		int x = long2tilex(position.x, zoom);
		int y = lat2tiley(position.y, zoom);
		Debug.Log("Fecthing image [" + server + "; " + zoom + "/" + x + "/" + y + "]");
		return "http://"+server+".tile.openstreetmap.org/"+zoom+"/"+x+"/"+y+".png";
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