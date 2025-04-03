using UnityEngine;
using System;
public interface IConsumableObjectTest
{

    public static event Action<string> testActionForConsumable;

    public static void InvokeConsumableObjectEvent(string testString)
    {
        testActionForConsumable?.Invoke(testString);
    }

    void OnConsumableObjectAction(string testString);
}