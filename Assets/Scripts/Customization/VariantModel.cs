using UnityEngine;

[CreateAssetMenu(fileName = "VariantModel", menuName = "MessageSeed/VariantModel", order = 1)]
public class VariantModel : ScriptableObject {

	[SerializeField] private Color _variantColor;
	[SerializeField] private Material _variantMaterial;

	public string VariantName => name;
	public Color VariantColor => _variantColor;
	public Material VariantMaterial => _variantMaterial;

}