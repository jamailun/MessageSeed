using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class VariantModelUI : MonoBehaviour {

	[SerializeField] private VariantModel model;

	private void Awake() {
		GetComponent<Image>().color = model.VariantColor;
	}

	private void Start() {
		
	}

	public void LocalClick() {
		Debug.Log("new color : " + model.name);
		DynamicAvatar.Instance?.ChangeColor(model);
	}

}