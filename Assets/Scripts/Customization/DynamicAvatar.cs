using UnityEngine;

public class DynamicAvatar : MonoBehaviour {
	[SerializeField] private MeshRenderer _renderer;

	public static DynamicAvatar Instance { get; private set; }

	[SerializeField] private VariantModel defaultModel;

	private void Awake() {
		Instance = this;
		// we don't care if we have another : it would be a proof i don't know how to code.
	}

	private void Start() {
		// Load asset name from local data
		string colorName = LocalData.GetPreferredModel();
		if(colorName == null) {
			ChangeColor(defaultModel);
			return;
		}
		// Load asset
		var asset = (VariantModel) UnityEditor.AssetDatabase.LoadAssetAtPath($"Assets/Resources/ModelVariant/{colorName}.asset", typeof(VariantModel));
		if(asset == null) {
			ChangeColor(defaultModel);
			return;
		}
		// Load it's color
		Debug.Log("Loading color : " + asset.name);
		ChangeColor(asset);
	}

	public void ChangeColor(VariantModel model) {
		_renderer.material = model.VariantMaterial;
	}
}