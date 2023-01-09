using UnityEngine;

[CreateAssetMenu(fileName = "VariantModel", menuName = "MessageSeed/VariantModel", order = 1)]
public class VariantModel : ScriptableObject {

	[SerializeField] private string _variantName;
	[SerializeField] private Color _variantColor;
	[SerializeField] private Material _variantMaterial;

	public string VariantName => _variantName;
	public Color VariantColor => _variantColor;
	public Material VariantMaterial => _variantMaterial;

}