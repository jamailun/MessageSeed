using UnityEngine;

public class PerspectivePan : MonoBehaviour {
    [SerializeField] private float groundZ = 0;
    [SerializeField] private float zoomOutMin = 1;
    [SerializeField] private float zoomOutMax = 8;
    [SerializeField] private float zoomSensitivity = 0.01f;

    public OnMoveEvent moveEvent;
    public OnZoomEvent zoomEvent;

    private Camera _camera;
    private Vector3 _touchStart;

	private void Start() {
        _camera = GetComponent<Camera>();
	}

	private void OnDrawGizmos() {
        Gizmos.color = Color.green;
        var b = CameraBounds;
        Gizmos.DrawWireCube(b.center, b.size);
	}

	void Update() {
        if(Input.GetMouseButtonDown(0)) {
            _touchStart = GetWorldPosition(groundZ);
        }

        if(Input.touchCount == 2) {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

            float difference = currentMagnitude - prevMagnitude;

            Zoom(difference * zoomSensitivity, true);
        } else if(Input.GetMouseButton(0)) {
            Vector3 direction = _touchStart - GetWorldPosition(groundZ);
            _camera.transform.position += direction;

            // call event
            moveEvent?.Invoke();
        }

        if(Input.GetKey(KeyCode.KeypadPlus)) {
            Zoom(zoomSensitivity, true);
        } else if(Input.GetKey(KeyCode.KeypadMinus)) {
            Zoom(-zoomSensitivity, true);
        }
    }

    private Vector3 GetWorldPosition(float z) {
        Ray mousePos = _camera.ScreenPointToRay(Input.mousePosition);
        Plane ground = new(Vector3.forward, new Vector3(0, 0, z));
		ground.Raycast(mousePos, out float distance);
		return mousePos.GetPoint(distance);
    }

    private float Zoom(float increment, bool triggersEvent) {
        float orthoSize = Mathf.Clamp(Camera.main.orthographicSize - increment, zoomOutMin, zoomOutMax);
        Camera.main.orthographicSize = orthoSize;

        // Call event
        if(triggersEvent)
            zoomEvent?.Invoke(orthoSize);
    
        return orthoSize;
    }

    public Bounds CameraBounds => Camera.main.OrthographicBounds();

    public delegate void OnMoveEvent();
    public delegate void OnZoomEvent(float zoom);
}