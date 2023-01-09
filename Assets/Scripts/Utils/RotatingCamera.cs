using UnityEngine;

public class RotatingCamera : MonoBehaviour {

	[SerializeField] private float speed = 2f;

	private void Update() {
		transform.Rotate(0, speed * Time.deltaTime, 0);
	}
}