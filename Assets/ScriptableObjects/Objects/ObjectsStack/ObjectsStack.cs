using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "ObjectsStack", menuName = "Scriptable Objects/ObjectsStack")]
public class ObjectsStack : ScriptableObject
{
    [SerializeField] public GameObject UIobjectPrefab;

    public List<AbstractObject> objects = new List<AbstractObject>();

    public static event Action addedFirstObjEvent;
    public static event Action removedLastObjEvent;

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

        if(objects.Count == 0)
        {
            addedFirstObjEvent?.Invoke();
        }
        objects.Add(obj);
    }
    public void RemoveObject(AbstractObject obj)
    {
        if(objects.Count == 1)
        {
            removedLastObjEvent?.Invoke();
        }
        objects.Remove(obj);

    }
    public void RemoveAllObjects()
    {
        objects.Clear();
    }
}
