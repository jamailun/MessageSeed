using UnityEngine;
using UnityEngine.Events;

public class MessageRaycast : MonoBehaviour {

	[SerializeField] private LayerMask layerMask;

	public MessageOpenedEvent messageOpenedEvent;

	private void Update() {
		bool click;
		if(Application.isMobilePlatform) {
			click = Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began;
		} else {
			click = Input.GetMouseButtonDown(0);
		}

		if(click) {
			Debug.LogWarning("TRY RAYCAST");
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			Debug.DrawRay(ray.origin, ray.direction.normalized * 1500f, Color.red, 3f);

			if(Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask)) {
				if(hit.collider.GetComponent<MessageRenderer>() != null) {
					Debug.Log("FIND MESSAGE !! " + hit.collider);
					MessageRenderer renderer = hit.collider.GetComponent<MessageRenderer>();
					renderer.OpenOrLoad(MessageReadyToOpen);
				}
				//Coordinates gpsCoordinates = Coordinates.convertVectorToCoordinates(hit.point);
			}
		}
	}

	private void MessageReadyToOpen(Message message) {
		messageOpenedEvent?.Invoke(message);
	}

	[System.Serializable]
	public class MessageOpenedEvent : UnityEvent<Message> { }


}