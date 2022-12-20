#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TestJson))]
public class TestJsonEditor : Editor {

	public override void OnInspectorGUI() {
		DrawDefaultInspector();
		EditorGUILayout.Space();

		if(GUILayout.Button("truc")) {
			var t = (TestJson) target;
			t.truc();
		}
		if(GUILayout.Button("machin")) {
			var t = (TestJson) target;
			t.machin();
		}

	}
}
#endif