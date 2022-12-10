#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AccountManager))]
public class AccountManagerEditor : Editor {

	public override void OnInspectorGUI() {
		DrawDefaultInspector();
		EditorGUILayout.Space();

		if(GUILayout.Button("TEST LOGIN")) {
			AccountManager am = (AccountManager) target;
			am.TryLogin("cochon", "ElleEstTropGros", null);
		}

	}
}
#endif