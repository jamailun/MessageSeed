using UnityEngine;

public class VersionDisplayer : MonoBehaviour {

	[SerializeField] private TMPro.TMP_Text field;

	void Start() {
		if(field)
			field.text = "Version " + Application.version;
	}

}