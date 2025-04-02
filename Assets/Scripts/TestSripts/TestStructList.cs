using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.Serialization;

public class TestStructList : MonoBehaviour
{

    [Serializable]
    private struct testStruct
    {
        public int id;
        public string name;
        public float value;
        [SerializeField] GameObject prefab; 
        [SerializeField] Idroppable idroppableinterface;
    }


    [SerializeField] private List<testStruct> testStructsList = new List<testStruct>();
}
