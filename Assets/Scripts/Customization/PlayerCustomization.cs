using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PlayerCustomization : MonoBehaviour {

	[Header("Common & default")]
	[SerializeField] private VariantModel defaultModel;

	[Header("Player choices")]
	[SerializeField] private VariantModel model;




	[System.Serializable]
	public class ModelChoiceChanged : UnityEvent<VariantModel> { }

}