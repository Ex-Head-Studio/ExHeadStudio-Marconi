using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "ObjectsStack", menuName = "Scriptable Objects/ObjectsStack")]
public class ObjectsStack : ScriptableObject
{
    [SerializeField] public GameObject UIobjectPrefab;

    public List<AbstractObject> objects = new List<AbstractObject>();

    private void OnEnable()
    {
        RemoveAllObjects();
    }
    private void OnDisable()
    {
        RemoveAllObjects();
    }
    public void AddObject(AbstractObject obj)
    {
        objects.Add(obj);
    }
    public void RemoveObject(AbstractObject obj)
    {
        objects.Remove(obj);
    }
    public void RemoveAllObjects()
    {
        objects.Clear();
    }
}
