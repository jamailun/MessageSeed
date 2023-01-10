using UnityEngine;
using UnityEngine.UI;

public class VariantSkyUI : MonoBehaviour {

	[SerializeField] private Image image;
	[SerializeField] private VariantSky model;
	public VariantSky Model => model;
	private PlayerCustomizationUI parent;

	private void Awake() {
		image.sprite = model.VariantSprite;
		parent = GetComponentInParent<PlayerCustomizationUI>();
	}

	public void LocalClick() {
		parent.ChangeSky(model);
	}

}