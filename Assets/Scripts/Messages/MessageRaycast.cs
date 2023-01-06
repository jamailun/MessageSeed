using UnityEngine;
using UnityEngine.Events;

public class MessageRaycast : MonoBehaviour {

	[SerializeField] private LayerMask layerMask;

	public MessageOpenedEvent messageOpenedEvent;

	private void Update() {
		bool drag;
		if(Application.isMobilePlatform) {
			drag = Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began;
		} else {
			drag = Input.GetMouseButton(0) && Input.GetAxisRaw("Mouse X") == 0.0f && Input.GetAxisRaw("Mouse Y") == 0.0f;
		}

		if(!drag) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if(Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask)) {
				if(hit.collider.GetComponent<MessageRenderer>() != null) {
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