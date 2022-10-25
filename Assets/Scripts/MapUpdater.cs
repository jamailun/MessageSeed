using UnityEngine;

public class MapUpdater : MonoBehaviour {

	[SerializeField] private MapRendererGrid mapRenderer;

	private double lastUpdate = 0;

	private void Update() {
		if(GpsPosition.Instance.LocationReady && GpsPosition.Instance.LastUpdate > lastUpdate) {
			lastUpdate = GpsPosition.Instance.LastUpdate;
			Debug.Log("Update (" + lastUpdate + ") : [" + GpsPosition.Instance.LastPosition.x + "; " + GpsPosition.Instance.LastPosition.y + "]");
			mapRenderer.SetPosition(GpsPosition.Instance.LastPosition);
			mapRenderer.UpdateMap();
		}
	}

}