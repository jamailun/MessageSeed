using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class MessageRaycast : MonoBehaviour {

	[SerializeField] private LayerMask layerMask;
	[SerializeField] private MainMenuUI guiManager;

	public MessageOpenedEvent messageOpenedEvent;

	private void Update() {
		bool click;
		if(Application.isMobilePlatform) {
			click = Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began;
		} else {
			click = Input.GetMouseButtonDown(0);
		}

		if(click && !guiManager.IsSomethingOpen()) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			Debug.DrawRay(ray.origin, ray.direction.normalized * 1500f, Color.red, 3f);

			// Raycast
			RaycastHit[] rays = Physics.RaycastAll(ray, 10000, layerMask);

			string s = "[";
			foreach(var r in rays)
				s += r.collider.name + ", ";
			Debug.LogWarning("RAYCAST SIZE : " + rays.Length+" => " + s + "]");
			
			// Found nothing : we early return
			if(rays.Length == 0)
				return;

			// Found 1 hit : we just display it
			if(rays.Length == 1) {
				MessageRenderer renderer = rays[0].collider.GetComponentInParent<MessageRenderer>();
				if(renderer)
					renderer.OpenOrLoad(MessageReadyToOpen);
				return;
			}

			// Found multiple elements, we display them in a nice UI
			List<MessageRenderer> renderers = new List<RaycastHit>(rays)
					.FindAll(hit => hit.collider.GetComponentInParent<MessageRenderer>() != null)
					.ConvertAll(hit => hit.collider.GetComponentInParent<MessageRenderer>());
			MessagesListWindow.Instance.OpenWithMessages(renderers, MessageReadyToOpen);
		}
	}

	private void MessageReadyToOpen(Message message) {
		Debug.Log("MESSAGE READY TO OPEN : " + message);
		messageOpenedEvent?.Invoke(message);
	}



	[System.Serializable]
	public class MessageOpenedEvent : UnityEvent<Message> { }


}