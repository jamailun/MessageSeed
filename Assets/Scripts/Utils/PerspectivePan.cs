using UnityEngine;

public class PerspectivePan : MonoBehaviour {

    public static PerspectivePan Instance { get; private set; }

    [SerializeField] private float groundZ = 0;
    [SerializeField] private float zoomOutMin = 1;
    [SerializeField] private float zoomOutMax = 8;
    [SerializeField] private float zoomSensitivity = 0.01f;
    [SerializeField] [Range(1f, 4f)] private float cameraBoundsBuffer = 1.5f;

    public OnMoveEvent moveEvent;
    public OnZoomEvent zoomEvent;

    private float _zoomInit;
    private Camera _camera;
    private Vector3 _touchStart;

    [SerializeField] [Range(1f, 5f)] private float _zoomThresholdMin = 1.5f;
    [SerializeField] [Range(5f, 10f)] private float _zoomThresholdMax = 6.5f;
    public float ZoomMinThreshold => _zoomThresholdMin;
    public float ZoomMaxThreshold => _zoomThresholdMax;

	private void Awake() {
		if(Instance) {
            Destroy(gameObject);
            return;
		}
        Instance = this;
	}

	private void Start() {
        _camera = GetComponent<Camera>();
        _zoomInit = _camera.orthographicSize;
    }

	private void OnDrawGizmos() {
        Gizmos.color = Color.green;
        var b = RawCameraBounds;
        Gizmos.DrawWireCube(b.center, b.size);
        Gizmos.color = Color.black;
        b = CameraBounds;
        Gizmos.DrawWireCube(b.center, b.size);
    }

	void Update() {
        if(Input.GetMouseButtonDown(0)) {
            _touchStart = GetUnityWorldPosition(groundZ);
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
            Vector3 direction = _touchStart - GetUnityWorldPosition(groundZ);
            Move(direction);
        }

#if UNITY_EDITOR
        if(Input.GetKey(KeyCode.KeypadPlus)) {
            Zoom(zoomSensitivity, true);
        } else if(Input.GetKey(KeyCode.KeypadMinus)) {
            Zoom(-zoomSensitivity, true);
        } else if(Input.GetKey(KeyCode.LeftArrow)) {
            Move(Vector2.left * Time.deltaTime);
        } else if(Input.GetKey(KeyCode.RightArrow)) {
            Move(Vector2.right * Time.deltaTime);
        } else if(Input.GetKey(KeyCode.UpArrow)) {
            Move(Vector2.up * Time.deltaTime);
        } else if(Input.GetKey(KeyCode.DownArrow)) {
            Move(Vector2.down * Time.deltaTime);
        }
#endif
    }

    // nothing to do with real world coordinates !
    private Vector3 GetUnityWorldPosition(float z) {
        Ray mousePos = _camera.ScreenPointToRay(Input.mousePosition);
        Plane ground = new(Vector3.forward, new Vector3(0, 0, z));
		ground.Raycast(mousePos, out float distance);
		return mousePos.GetPoint(distance);
    }

	private void Move(Vector3 direction) {
        _camera.transform.position += direction;
        // call event
        moveEvent?.Invoke();
    }

	private float Zoom(float increment, bool triggersEvent) {
        float orthoSize = Mathf.Clamp(Camera.main.orthographicSize - increment, zoomOutMin, zoomOutMax);
        Camera.main.orthographicSize = orthoSize;

        // Call event
        if(triggersEvent)
            zoomEvent?.Invoke(orthoSize);
    
        return orthoSize;
    }

    public void ResetZoom() {
        Camera.main.orthographicSize = _zoomInit;
    }

    public Bounds CameraBounds => new(RawCameraBounds.center, RawCameraBounds.size * cameraBoundsBuffer);

    public Bounds RawCameraBounds => Camera.main.OrthographicBounds();

    public delegate void OnMoveEvent();
    public delegate void OnZoomEvent(float zoom);
}