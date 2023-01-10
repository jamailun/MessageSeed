using UnityEngine;

[CreateAssetMenu(fileName = "VariantSky", menuName = "MessageSeed/VariantSky", order = 2)]
[System.Serializable]
public class VariantSky : ScriptableObject {

	[SerializeField] private Sprite _variantSprite;
	[SerializeField] private Material _variantMaterial;

	public string VariantName => name;
	public Sprite VariantSprite => _variantSprite;
	public Material VariantMaterial => _variantMaterial;

}