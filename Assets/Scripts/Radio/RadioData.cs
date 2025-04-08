using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using FMOD;


[CreateAssetMenu(fileName = "RadioData", menuName = "Scriptable Objects/RadioData")]
public class RadioData : ScriptableObject
{
    [SerializeField] public EventReference[] radioEventReferences;
}
