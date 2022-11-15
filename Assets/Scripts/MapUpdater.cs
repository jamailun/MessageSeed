using UnityEngine;

public class MapUpdater : MonoBehaviour {

	[SerializeField] private MapRendererGrid _mapRenderer;

	private double _lastUpdate = 0;

	private void Update() {
		// changement de position
		if(GpsPosition.Instance.LocationReady && GpsPosition.Instance.LastUpdate > _lastUpdate) {
			_lastUpdate = GpsPosition.Instance.LastUpdate;
			Debug.Log("Update (" + _lastUpdate + ") : [" + GpsPosition.Instance.LastPosition.x + "; " + GpsPosition.Instance.LastPosition.y + "]");
			_mapRenderer.SetPosition(GpsPosition.Instance.LastPosition);
			_mapRenderer.UpdateMap();
		}
	}

}