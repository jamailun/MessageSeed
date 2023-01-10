using UnityEngine;

public class DynamicAvatar : MonoBehaviour {
	[SerializeField] private MeshRenderer _renderer;

	public static DynamicAvatar Instance { get; private set; }

	private void Awake() {
		Instance = this;
		// we don't care if we have another : it would be a proof i don't know how to code.
	}

	public void ChangeColor(VariantModel model) {
		_renderer.material = model.VariantMaterial;
	}
}