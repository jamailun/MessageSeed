using UnityEngine;

public static class CSharpExtenstion {

	/// <summary>
	/// Destroy all children of a gameobject in the hierarchy.
	/// </summary>
	/// <param name="transform">The transform of the parent.</param>
	/// <param name="immediate">if true, DestroyImmediate will be called.</param>
	public static void DestroyChildren(this Transform transform) {
		foreach(Transform child in transform) {
			Object.Destroy(child.gameObject);
		}
	}

	/// <summary>
	/// Get or add a component on a gameobject.
	/// </summary>
	/// <typeparam name="T">The component to add</typeparam>
	/// <param name="obj">The object to get/add the component from.</param>
	/// <returns>The get/created component.</returns>
	public static T GetOrAddComponent<T>(this GameObject obj) where T : Component {
		var cp = obj.GetComponent<T>();
		if(cp == null)
			return obj.AddComponent<T>();
		return cp;
	}
	public static Bounds OrthographicBounds(this Camera camera) {
#if UNITY_EDITOR
		return new(camera.transform.position, camera.orthographicSize * new Vector3(1f , 2f));
#else
		float screenAspect = (float) Screen.currentResolution.width / Screen.currentResolution.height;
		float cameraHeight = camera.orthographicSize * 2;
		return new(
			camera.transform.position,
			new Vector3(cameraHeight * screenAspect, cameraHeight, 0)
		);
#endif
	}

	public static Bounds Expand(this Bounds that, Bounds bounds) {
		Vector3 min = new(
			Mathf.Min(that.min.x, bounds.min.x),
			Mathf.Min(that.min.y, bounds.min.y),
			Mathf.Min(that.min.z, bounds.min.z)
		);
		Vector3 max = new(
		  Mathf.Max(that.max.x, bounds.max.x),
		  Mathf.Max(that.max.y, bounds.max.y),
		  Mathf.Max(that.max.z, bounds.max.z)
		);
		Vector3 size = (max - min);
		Vector3 center = min + size / 2f;

		return new(center, size);
	}

	public static bool Overlaps2D(this Bounds that, Bounds bounds) {
		return that.min.x <= bounds.min.x && that.max.x >= bounds.max.x
			&& that.min.y <= bounds.min.y && that.max.y >= bounds.max.y;
	}

}	