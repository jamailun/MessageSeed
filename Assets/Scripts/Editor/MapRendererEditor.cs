using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapRenderer))]
public class GoogleMapDisplayEditor : Editor {

	public override void OnInspectorGUI() {
		DrawDefaultInspector();
		EditorGUILayout.Space();

		if(GUILayout.Button("Update map image")) {
			MapRenderer gmap = (MapRenderer) target;
			gmap.ReloadProvider();
			gmap.UpdateMap();
		}
	}
}