using System.Collections.Generic;
using UnityEngine;

public class PlayerCustomizationUI : MonoBehaviour {

	private readonly Dictionary<string, VariantModelUI> buttons = new();
	[SerializeField] private GameObject validCursor;
	[SerializeField] private VariantModel defaultModel;

	private void Start() {
		foreach(var model in GetComponentsInChildren<VariantModelUI>()) {
			buttons.Add(model.Model.name, model);
		}
		LoadModel();
	}

	private void LoadModel() {
		// Load asset name from local data
		string colorName = LocalData.GetPreferredModel();
		if(colorName == null) {
			ChangeColor(defaultModel);
			return;
		}
		// Load asset
		var asset = Resources.Load<VariantModel>("ModelVariant/" + colorName);
		if(asset == null) {
			ChangeColor(defaultModel);
			return;
		}
		// Load it's color
		Debug.Log("Loaded model color : " + asset.name);
		ChangeColor(asset);
	}

	public void ChangeColor(VariantModel model) {
		DynamicAvatar.Instance.ChangeColor(model);
		validCursor.transform.SetParent(buttons[model.name].transform, false);
		LocalData.SavePreferredModel(model);
	}



}