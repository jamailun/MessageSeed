using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapRendererGrid))]
public class MapRendererGridEditor : Editor {

	public override void OnInspectorGUI() {
		DrawDefaultInspector();
		EditorGUILayout.Space();

		if(GUILayout.Button("Update all")) {
			MapRendererGrid gmap = (MapRendererGrid) target;
			gmap.ReloadUrlFetcher();
			gmap.UpdateChildren();
			gmap.UpdateMap();
		}

		if(GUILayout.Button("Update children only")) {
			MapRendererGrid gmap = (MapRendererGrid) target;
			gmap.UpdateChildren();
		}

		if(GUILayout.Button("Update image only")) {
			MapRendererGrid gmap = (MapRendererGrid) target;
			gmap.UpdateMap();
		}
	}
}