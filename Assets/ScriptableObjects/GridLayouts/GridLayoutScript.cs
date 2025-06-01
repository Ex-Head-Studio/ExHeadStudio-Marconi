using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections;



//Classe wrapper di un array generico
[System.Serializable]
public class Wrapper<T>
{
    public T[] values;
}


[CreateAssetMenu(fileName = "GridLayoutSO", menuName = "Grid Layout")]
public class GridLayoutScript : ScriptableObject
{
    [HideInInspector]
    public Wrapper<GameObject>[] grid;

    [Header("Grid Settings")]
    [SerializeField] public static int size = 6; // Size of the grid (size x size)
    [SerializeField] public GameObject[] objects; // Array of objects to be placed in the grid

    private int objectsLength;
    private void Awake()
    {
        if (grid == null)
        {
            ResetGrid();
        }
    }
    public void ResetGrid()
    {
        grid = new Wrapper<GameObject>[size];
        for (int i = 0; i < size; i++)
        {
            grid[i] = new Wrapper<GameObject>();
            grid[i].values = new GameObject[size];

            for (int j = 0; j < size; j++)
            {
                // Initialize each cell with a random object from the objects array
                if (objects != null && objects.Length > 0)
                {
                    grid[i].values[j] = objects[0];
                }
                else
                {
                    grid[i].values[j] = null; // or assign a default object if needed
                }
            }
        }
    }

    public void SetObject(int x, int y)
    {
        int currentIndex = System.Array.IndexOf(objects, grid[x].values[y]);
        grid[x].values[y] = objects[NextIndex(currentIndex)];
    }

    private int NextIndex(int index)
    {
        int res = (index+1) % objects.Length;
        return res;
    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(GridLayoutScript))]
public class GridLayoutScriptEditor : Editor
{
    GridLayoutScript script;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();
        script = (GridLayoutScript)target;
        if (script.objects == null || script.objects[0] == null)
        {
            EditorGUILayout.HelpBox("No objects assigned to the grid. Please assign objects in the inspector.", UnityEditor.MessageType.Warning);
        }
        else if(script.objects[0] != null && script.objects.Length == 1)
        {
            script.ResetGrid();
            DrawGrid();
        }
        else if (script.objects.Length > 1)
        {
            DrawGrid();
        }


        if (GUILayout.Button("Reset Grid") && script.objects.Length > 0)
        {
            script.ResetGrid();
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawGrid()
    {
        try
        {
            GUILayout.BeginVertical();
            for (int i = 0; i < GridLayoutScript.size; i++)
            {
                GUILayout.BeginHorizontal();

                for (int j = 0; j < GridLayoutScript.size; j++)
                {
                    if (GUILayout.Button( script.grid[i].values[j] != null ? script.grid[i].values[j].name : "Empty", GUILayout.Width(80), GUILayout.Height(50)))
                    {
                        script.SetObject(i, j);
                        serializedObject.ApplyModifiedProperties();
                        EditorUtility.SetDirty(script); // Mark the scriptable object as dirty to save changes
                    }
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);           
        }
    }
}

#endif

