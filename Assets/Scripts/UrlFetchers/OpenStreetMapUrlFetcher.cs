using UnityEngine;

public class OpenStreetMapURL : UrlFetcher {

	private static readonly string[] SERVER = {"a", "b", "c"};
	private static string GetServer() { return SERVER[UnityEngine.Random.Range(0, 2)]; }

	private readonly string server;
	public OpenStreetMapURL() {
		server = GetServer();
	}

	public string CreateUrl(double latitude, double longitude, int zoom = 10) {
		Vector2Int tile = MapUtils.GetTile(longitude, latitude, zoom);
		Debug.Log("Fecthing image ["+server+"; z=" + zoom + "; pos=(" + tile.x + ";" + tile.y + ")");
		return "http://"+ server + ".tile.openstreetmap.org/"+zoom+"/"+tile.x+"/"+tile.y+".png";
	}

	public string CreateUrlTile(int x, int y, int zoom) {
		return "http://" + server + ".tile.openstreetmap.org/" + zoom + "/" + x + "/" + y + ".png";
	}


}