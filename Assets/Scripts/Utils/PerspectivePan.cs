using UnityEngine;

public class PerspectivePan : MonoBehaviour {
    [SerializeField] private float groundZ = 0;

    private Camera _camera;
    private Vector3 _touchStart;

	private void Start() {
        _camera = GetComponent<Camera>();
	}

	void Update() {
        if(Input.GetMouseButtonDown(0)) {
            _touchStart = GetWorldPosition(groundZ);
        }
        if(Input.GetMouseButton(0)) {
            Vector3 direction = _touchStart - GetWorldPosition(groundZ);
            _camera.transform.position += direction;
        }
    }

    private Vector3 GetWorldPosition(float z) {
        Ray mousePos = _camera.ScreenPointToRay(Input.mousePosition);
        Plane ground = new(Vector3.forward, new Vector3(0, 0, z));
		ground.Raycast(mousePos, out float distance);
		return mousePos.GetPoint(distance);
    }
}