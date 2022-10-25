using System.Collections;
using UnityEngine;

public class GpsPosition : MonoBehaviour {

    public static GpsPosition Instance { get; private set; }

    [Tooltip("Duration between two updates of position")]
    [Range(1f, 5f)] [SerializeField] private float updatePositionAfter = 2f;

    public bool HasLocationEnabled { get; private set; }
    public bool LocationReady { get; private set; }
    public Vector2 LastPosition { get; private set; }
    public double LastUpdate { get; private set; }

	private void Awake() {
		// Singleton check
        if(Instance != null) {
            Destroy(gameObject);
            return;
		}
        Instance = this;
        DontDestroyOnLoad(gameObject);
	}

	private void Start() {
        HasLocationEnabled = Input.location.isEnabledByUser;
        if(!HasLocationEnabled) {
            Debug.LogError("Cannot access localisation data.");
            return;
		}
        LocationReady = false;
        StartCoroutine(GetPositionLoop());
    }

	private void OnDisable() {
        // Shutdown location on disabling
        if(LocationReady) {
            Input.location.Stop();
            Debug.Log("Location service shutdown.");
        }
	}

	private IEnumerator GetPositionLoop() {
        // Starts the location service.
        Input.location.Start();

        // Waits until the location service initializes
        int maxWait = 20;
        while(Input.location.status == LocationServiceStatus.Initializing && maxWait > 0) {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // If the service didn't initialize in 20 seconds this cancels location service use.
        if(maxWait < 1) {
            Debug.LogError("Location initialisation timed out");
            yield break;
        }

        // If the connection failed this cancels location service use.
        if(Input.location.status == LocationServiceStatus.Failed) {
            Debug.LogError("Unable to determine device location");
            yield break;
        }

        Debug.Log("Location service started successfully. (" + Input.location.status + ")");
        LocationReady = true;

        while(Input.location.status == LocationServiceStatus.Running) {
            yield return new WaitForSeconds(updatePositionAfter);
            LastPosition = new(Input.location.lastData.latitude, Input.location.lastData.longitude);
            LastUpdate = Input.location.lastData.timestamp;
            Debug.Log("Position = "+LastPosition+" at " + LastUpdate+".");
        }

        LocationReady = false;
        Debug.LogWarning("Location service has been disabled (" + Input.location.status + ").");
    }
}