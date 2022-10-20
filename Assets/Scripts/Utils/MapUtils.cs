using UnityEngine;
using System;

public class MapUtils {

	public static double ToRadians(double angle) {
		return (Math.PI / 180d) * angle;
	}

	public static Vector2Int GetTile(double latitude, double longitude, int zoom) {
		Vector2Int vec = new();

		double lat_rad = ToRadians(latitude);
		double n = Math.Pow(2d, zoom);
		vec.x = (int) Math.Floor((longitude + 180d) / 360d * n);
		vec.y = (int) Math.Floor((1.0 - Math.Asinh(Math.Tan(lat_rad)) / Math.PI) / 2d * n);

		return vec;
	}

	public static int Longitude2ToTileX(double lon, int z) {
		return (int) (Math.Floor((lon + 180.0) / 360.0 * (1 << z)));
	}
	// DOESNT WORK
	public static int LatitudeToTileY(double lat, int z) {
		double latRad = ToRadians(lat);
		double d1 = Math.Tan(latRad) + (1d / Math.Cos(latRad));
		double d2 = Math.Log(d1);
		double d3 = d2 / Math.PI;
		int zz = 1 << z;
		double val = (1 - d3) / 2 * zz;
		Debug.Log("latRad=[" + latRad + "], zz=[" + zz + "], d1=[" + d1 + "], d2=[" + d2 + "], d3=[" + d3 + "], val=[" + val + "], valFloor=[" + Math.Floor(val) + "], valFloorInt=[" + ((int) Math.Floor(val)) + "].");
		return (int) Math.Floor(val);
	}
}
