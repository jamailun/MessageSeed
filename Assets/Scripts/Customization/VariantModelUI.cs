using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class VariantModelUI : MonoBehaviour {

	[SerializeField] private VariantModel model;
	public VariantModel Model => model;
	private PlayerCustomizationUI parent;

	private void Awake() {
		GetComponent<Image>().color = model.VariantColor;
		parent = GetComponentInParent<PlayerCustomizationUI>();
	}

	public void LocalClick() {
		Debug.Log("new color : " + model.name);
		parent.ChangeColor(model);
	}

}