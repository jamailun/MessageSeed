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

}	