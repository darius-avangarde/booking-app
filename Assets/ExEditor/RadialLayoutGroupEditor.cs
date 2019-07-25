#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(RadialLayoutGroup)), CanEditMultipleObjects]
public class RadialLayoutGroupEditor : Editor
{
	private bool activeUpdate = false;
	private string buttonText = "Enable Alignment";
	private RadialLayoutGroup script;

	public override void OnInspectorGUI()
	{

		EditorGUILayout.LabelField ("Should be on button group parent", GUI.skin.textField);
		script = (RadialLayoutGroup)target;

		if (activeUpdate)
		{
			GUILayout.BeginVertical ("", GUI.skin.textField);
				DrawDefaultInspector ();
			GUILayout.EndVertical ();

			GUILayout.BeginVertical ("", GUI.skin.textField);
				script.separationY = EditorGUILayout.FloatField ("spacing Y", script.separationY);

				if (!script.useDegres)
				{
					script.separationX = EditorGUILayout.FloatField ("spacing X", script.separationX);
				}
				else
				{
					script.separationXdeg = EditorGUILayout.FloatField ("spacing X °", script.separationXdeg);
				}
			GUILayout.EndVertical ();
		}

		GUILayout.BeginHorizontal ("", GUI.skin.textField);
			if (GUILayout.Button(buttonText))
			{
				activeUpdate = !activeUpdate;
				buttonText = (activeUpdate) ? "Disable Alignment" : "Enable Alignment";
			}
		GUILayout.EndHorizontal ();


		if (activeUpdate && Selection.activeTransform == script.transform)
		{
			script.PlaceButtons ();
		}
	}
}
#endif
