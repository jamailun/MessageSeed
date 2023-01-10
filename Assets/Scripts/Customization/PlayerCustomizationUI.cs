using System.Collections.Generic;
using UnityEngine;

public class PlayerCustomizationUI : MonoBehaviour {

	private readonly Dictionary<string, VariantModelUI> buttonsColor = new();
	private readonly Dictionary<string, VariantSkyUI> buttonsSky = new();

	[Header("Cursors")]
	[SerializeField] private GameObject validCursorColor;
	[SerializeField] private GameObject validCursorSky;

	[Header("Default values")]
	[SerializeField] private VariantModel defaultColor;
	[SerializeField] private VariantSky defaultSky;

	private void Start() {
		foreach(var model in GetComponentsInChildren<VariantModelUI>())
			buttonsColor.Add(model.Model.name, model);

		foreach(var model in GetComponentsInChildren<VariantSkyUI>())
			buttonsSky.Add(model.Model.name, model);

		LoadColor();
	}

	private void LoadColor() {
		// Load asset name from local data
		string colorName = LocalData.GetPreferredModel();
		if(colorName == null) {
			ChangeColor(defaultColor);
			return;
		}
		// Load asset
		var asset = Resources.Load<VariantModel>("ModelVariant/" + colorName);
		if(asset == null) {
			ChangeColor(defaultColor);
			return;
		}
		// Load it's color
		Debug.Log("Loaded model color : " + asset.name);
		ChangeColor(asset);
	}

	private void LoadSky() {
		// Load asset name from local data
		string skyName = LocalData.GetPreferredSky();
		if(skyName == null) {
			ChangeSky(defaultSky);
			return;
		}
		// Load asset
		var asset = Resources.Load<VariantSky>("SkyVariant/" + skyName);
		if(asset == null) {
			ChangeSky(defaultSky);
			return;
		}
		// Load it's color
		Debug.Log("Loaded model sky : " + asset.name);
		ChangeSky(asset);
	}

	public void ChangeColor(VariantModel model) {
		DynamicAvatar.Instance.ChangeColor(model);
		validCursorColor.transform.SetParent(buttonsColor[model.name].transform, false);
		LocalData.SavePreferredModel(model);
	}

	public void ChangeSky(VariantSky model) {
		RenderSettings.skybox = model.VariantMaterial;
		validCursorSky.transform.SetParent(buttonsSky[model.name].transform, false);
		LocalData.SavePreferredSky(model);
	}



}