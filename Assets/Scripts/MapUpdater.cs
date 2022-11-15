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
		// check si la caméra est contenu dans la grid existante
		Bounds camera = _perspectiveController.CameraBounds;
		Bounds grid = _mapRenderer.CurrentBounds;
		if(grid.Overlaps2D(camera)) {
			Debug.Log("comaera complitely inside grid.");
		}
		// si oui, ba on change rien : le mouvement est mineur

		// si non, il faut charger de nouvelles tiles.
	}

	private void UserZoomed(float v) {
		
	}

}