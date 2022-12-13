using UnityEngine;

public class MapUpdater : MonoBehaviour {

	[SerializeField] private MapRendererGrid _mapRenderer;

	private double _lastUpdate = 0;


	private void Start() {
		PerspectivePan.Instance.moveEvent += UserMoved;
		PerspectivePan.Instance.zoomEvent += UserZoomed;
		// on startup, init renderers
		PostModification();
	}

	private void OnDisable() {
		PerspectivePan.Instance.moveEvent -= UserMoved;
		PerspectivePan.Instance.zoomEvent -= UserZoomed;
	}

	private void Update() {
		// changement de position
		if(GpsPosition.Instance.LocationReady && GpsPosition.Instance.LastUpdate > _lastUpdate) {
			_lastUpdate = GpsPosition.Instance.LastUpdate;
			Debug.Log("Update (" + _lastUpdate + ") : [" + GpsPosition.Instance.LastPosition.x + "; " + GpsPosition.Instance.LastPosition.y + "]");
			_mapRenderer.SetPosition(GpsPosition.Instance.LastPosition);
			_mapRenderer.UpdateMap();
		}
	}

	private void UserMoved() {
		// Simply check camera can see every tiles
		PostModification();
	}

	private void UserZoomed(float z) {
		// nouvelle valeur du zoom en int ??
		Debug.Log("user zoomed = " + z);
		if(z <= PerspectivePan.Instance.ZoomMinThreshold) {
			//Zoom in !
			_mapRenderer.ZoomLayerChange(true);
			PerspectivePan.Instance.ResetZoom();
		} else if(z >= PerspectivePan.Instance.ZoomMaxThreshold) {
			//Zoom out !
			_mapRenderer.ZoomLayerChange(false);
			PerspectivePan.Instance.ResetZoom();
		} else {
			// simple check
			PostModification();
		}
	}

	private void PostModification() {
		// check si la caméra est contenu dans la grid existante
		Bounds camera = PerspectivePan.Instance.CameraBounds;
		Bounds grid = _mapRenderer.CurrentBounds;

		// Camera not completely inside grid : update renderers
		if(! grid.Contains2D(camera)) {
			_mapRenderer.UpdateGridVisibility();
		}
	}

}