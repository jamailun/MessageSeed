using UnityEngine;

public class MapUpdater : MonoBehaviour {

	[SerializeField] private MapRendererGrid _mapRenderer;
	[SerializeField] private PerspectivePan _perspectiveController;

	private double _lastUpdate = 0;

	#region events registration
	private void OnEnable() {
		_perspectiveController.moveEvent += UserMoved;
		_perspectiveController.zoomEvent += UserZoomed;
	}

	private void OnDisable() {
		_perspectiveController.moveEvent -= UserMoved;
		_perspectiveController.zoomEvent -= UserZoomed;
	}
	#endregion

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
		// pour l'instant, on vérifie que l'utilisateur ait toujours tout en face.
		PostModification();
	}

	private void UserZoomed(float z) {
		// nouvelle valeur du zoom en int ??
		Debug.Log("new zoom = " + z);
		// pour l'instant, on vérifie que l'utilisateur ait toujours tout en face.
		PostModification();
	}

	private void PostModification() {
		// check si la caméra est contenu dans la grid existante
		Bounds camera = _perspectiveController.CameraBounds;
		Bounds grid = _mapRenderer.CurrentBounds;
		if(! grid.Contains2D(camera)) {
			//Debug.Log("camera NOT completely inside grid.");
			_mapRenderer.UpdateGridVisibility(camera);
		}
	}

}