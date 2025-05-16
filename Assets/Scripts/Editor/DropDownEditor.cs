using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Linq;

[CustomEditor(typeof(ChangeStatsEffect))]
public class DropDownEditor : Editor
{
    private int selectedIndex;
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        /*
        ChangeStatsEffect script = (ChangeStatsEffect)target;
        serializedObject.Update();
        EditorGUILayout.PrefixLabel("Attributes");

        selectedIndex = EditorGUILayout.Popup(selectedIndex, script.shipSO.statNames);
        script.arrayIdx = selectedIndex;

        serializedObject.ApplyModifiedProperties();



        /*GUIContent arrayLabel = new GUIContent("MyArray");
        script.arrayIdx = EditorGUILayout.Popup(arrayLabel, script.arrayIdx, script.shipSO.statNames);*/

    }
}